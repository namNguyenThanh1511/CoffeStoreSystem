using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.Repositories;
using PRN232.Lab2.CoffeeStore.Repositories.Entities;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;
using PRN232.Lab2.CoffeeStore.Services.Exceptions;
using PRN232.Lab2.CoffeeStore.Services.Models;
using PRN232.Lab2.CoffeeStore.Services.Models.Order;
using PRN232.Lab2.CoffeeStore.Services.Models.Product;
using PRN232.Lab2.CoffeeStore.Services.PaymentService;
using PRN232.Lab2.CoffeeStore.Services.UserService;

namespace PRN232.Lab2.CoffeeStore.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPaymentService _paymentService;


        public OrderService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _paymentService = paymentService;

        }

        public async Task<(List<OrderPlacingResponse>, MetaData metaData)> GetAllOrders(OrderSearchParams searchParams)
        {
            (string userId, string role) = _currentUserService.GetCurrentUser();

            var query = _unitOfWork.Orders.Query();

            // Filtering
            if (!string.IsNullOrWhiteSpace(searchParams.Search))
            {
                query = query.Where(o =>
                    o.Status.ToString().Contains(searchParams.Search) ||
                    (o.Customer != null && o.Customer.FullName.Contains(searchParams.Search))
                );
            }

            // Restrict to current user if not admin
            if (role != Role.Admin.ToString() && role != Role.Barista.ToString())
            {
                query = query.Where(o => o.CustomerId == Guid.Parse(userId));
            }

            // Sorting
            string sortBy = string.IsNullOrWhiteSpace(searchParams.SortBy) ? "OrderDate" : searchParams.SortBy;
            string sortOrder = string.IsNullOrWhiteSpace(searchParams.SortOrder) ? "desc" : searchParams.SortOrder.ToLower();

            switch (sortBy.ToLower())
            {
                case "orderdate":
                    query = sortOrder == "asc" ? query.OrderBy(o => o.OrderDate) : query.OrderByDescending(o => o.OrderDate);
                    break;
                case "totalamount":
                    query = sortOrder == "asc" ? query.OrderBy(o => o.TotalAmount) : query.OrderByDescending(o => o.TotalAmount);
                    break;
                case "status":
                    query = sortOrder == "asc" ? query.OrderBy(o => o.Status) : query.OrderByDescending(o => o.Status);
                    break;
                default:
                    query = sortOrder == "asc" ? query.OrderBy(o => o.OrderDate) : query.OrderByDescending(o => o.OrderDate);
                    break;
            }

            //filtering by Statuses
            if (searchParams.Statuses != null && searchParams.Statuses.Count > 0)
            {
                var statusEnums = searchParams.Statuses
                    .Select(s => Enum.Parse<OrderStatus>(s, true))
                    .ToList();
                query = query.Where(o => statusEnums.Contains(o.Status));
            }
            //filtering by PaymentStatuses
            if (searchParams.PaymentStatuses != null && searchParams.PaymentStatuses.Count > 0)
            {
                var paymentStatusEnums = searchParams.PaymentStatuses
                    .Select(s => Enum.Parse<PaymentStatus>(s, true))
                    .ToList();
                query = query.Where(o => o.PaymentStatus != null && paymentStatusEnums.Contains(o.PaymentStatus));
            }
            //filtering by DeliveryTypes
            if (searchParams.DeliveryTypes != null && searchParams.DeliveryTypes.Count > 0)
            {
                var deliveryTypeEnums = searchParams.DeliveryTypes
                    .Select(s => Enum.Parse<DeliveryType>(s, true))
                    .ToList();
                query = query.Where(o => o.DeliveryType != null && deliveryTypeEnums.Contains(o.DeliveryType));
            }

            // Include navigation properties before passing to repository
            query = query.Include(o => o.OrderItems)
                         .ThenInclude(oi => oi.Variant)
                            .ThenInclude(v => v.Product)
                         .Include(o => o.Customer);
            query = query.Include(o => o.OrderItems)
                         .ThenInclude(oi => oi.OrderItemAddons)
                         .ThenInclude(oia => oia.Addon);

            // Use repository for paging
            var pagedOrders = await _unitOfWork.Orders.GetAllOrders(query, searchParams.PageNumber, searchParams.PageSize);

            // Select only requested fields
            var selectFields = searchParams.SelectFields;

            var result = pagedOrders.Select(order =>
            {
                var response = new OrderPlacingResponse
                {
                    Id = order.Id
                };

                // If no select fields specified, populate all fields
                if (selectFields == null || selectFields.Count == 0)
                {
                    response.OrderDate = order.OrderDate;
                    response.Status = order.Status.ToString();
                    response.TotalAmount = order.TotalAmount;
                    response.CustomerId = order.CustomerId ?? Guid.Empty;
                    response.OrderItems = order.OrderItems.Select(oi => new OrderItemResponse
                    {
                        Id = oi.Id,
                        ProductsWithVariant = oi.Variant?.Product != null ? new ProductWithVariantResponse
                        {
                            ProductId = oi.Variant.Product.Id,
                            ProductName = oi.Variant.Product.Name,
                            VariantId = oi.Variant.Id,
                            VariantSize = oi.Variant.Size.ToString(),
                            BasePrice = oi.Variant.BasePrice
                        } : null,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Notes = oi.Notes,
                        Temperature = oi.Temperature.ToString(),
                        Sweetness = oi.Sweetness.ToString(),
                        MilkType = oi.MilkType.ToString(),
                        Addons = oi.OrderItemAddons?.Select(oia => new CoffeeAddonResponse
                        {
                            Id = oia.AddonId,
                            Name = oia.Addon != null ? oia.Addon.Name : "",
                            Price = oia.Price
                        }).ToList() ?? new List<CoffeeAddonResponse>()
                    }).ToList();
                }
                else
                {
                    // Always include Id, populate only selected fields
                    foreach (var field in selectFields)
                    {
                        switch (field)
                        {
                            case OrderSearchParams.OrderSelectField.OrderDate:
                                response.OrderDate = order.OrderDate ?? null;
                                break;
                            case OrderSearchParams.OrderSelectField.Status:
                                response.Status = order.Status.ToString();
                                break;
                            case OrderSearchParams.OrderSelectField.TotalAmount:
                                response.TotalAmount = order.TotalAmount;
                                break;
                            case OrderSearchParams.OrderSelectField.Customer:
                                response.CustomerId = order.CustomerId ?? null;
                                break;
                            case OrderSearchParams.OrderSelectField.OrderItems:
                                response.OrderItems = order.OrderItems.Select(oi => new OrderItemResponse
                                {
                                    Id = oi.Id,
                                    //ProductId = oi.ProductId,
                                    //ProductName = oi.Product?.Name ?? "",
                                    Quantity = oi.Quantity,
                                    UnitPrice = oi.UnitPrice
                                }).ToList();
                                break;
                        }
                    }
                }

                return response;
            });

            return (result.ToList(), pagedOrders.MetaData);
        }
        public async Task<(List<OrderPlacingResponse>, MetaData metaData)> GetOrdersByCurrentUser(OrderSearchParams searchParams)
        {
            (string userId, string role) = _currentUserService.GetCurrentUser();

            // Check role is Customer
            if (role != Role.Customer.ToString())
            {
                throw new UnauthorizedAccessException("Only customers can access their own orders.");
            }

            var customerId = Guid.Parse(userId);
            var query = _unitOfWork.Orders.Query();

            // Filter by current user's CustomerId
            query = query.Where(o => o.CustomerId == customerId);

            // Filtering by search term
            if (!string.IsNullOrWhiteSpace(searchParams.Search))
            {
                query = query.Where(o =>
                    o.Status.ToString().Contains(searchParams.Search) ||
                    o.OrderCode.ToString().Contains(searchParams.Search) ||
                    (o.Customer != null && o.Customer.FullName.Contains(searchParams.Search))
                );
            }

            // Sorting
            string sortBy = string.IsNullOrWhiteSpace(searchParams.SortBy) ? "OrderDate" : searchParams.SortBy;
            string sortOrder = string.IsNullOrWhiteSpace(searchParams.SortOrder) ? "desc" : searchParams.SortOrder.ToLower();

            switch (sortBy.ToLower())
            {
                case "orderdate":
                    query = sortOrder == "asc" ? query.OrderBy(o => o.OrderDate) : query.OrderByDescending(o => o.OrderDate);
                    break;
                case "totalamount":
                    query = sortOrder == "asc" ? query.OrderBy(o => o.TotalAmount) : query.OrderByDescending(o => o.TotalAmount);
                    break;
                case "status":
                    query = sortOrder == "asc" ? query.OrderBy(o => o.Status) : query.OrderByDescending(o => o.Status);
                    break;
                default:
                    query = sortOrder == "asc" ? query.OrderBy(o => o.OrderDate) : query.OrderByDescending(o => o.OrderDate);
                    break;
            }


            //filtering by Statuses
            if (searchParams.Statuses != null && searchParams.Statuses.Count > 0)
            {
                var statusEnums = searchParams.Statuses
                    .Select(s => Enum.Parse<OrderStatus>(s, true))
                    .ToList();
                query = query.Where(o => statusEnums.Contains(o.Status));
            }
            //filtering by PaymentStatuses
            if (searchParams.PaymentStatuses != null && searchParams.PaymentStatuses.Count > 0)
            {
                var paymentStatusEnums = searchParams.PaymentStatuses
                    .Select(s => Enum.Parse<PaymentStatus>(s, true))
                    .ToList();
                query = query.Where(o => o.PaymentStatus != null && paymentStatusEnums.Contains(o.PaymentStatus));
            }
            //filtering by DeliveryTypes
            if (searchParams.DeliveryTypes != null && searchParams.DeliveryTypes.Count > 0)
            {
                var deliveryTypeEnums = searchParams.DeliveryTypes
                    .Select(s => Enum.Parse<DeliveryType>(s, true))
                    .ToList();
                query = query.Where(o => o.DeliveryType != null && deliveryTypeEnums.Contains(o.DeliveryType));
            }


            // Include navigation properties before passing to repository
            query = query.Include(o => o.OrderItems)
                         .ThenInclude(oi => oi.Variant)
                            .ThenInclude(v => v.Product)
                         .Include(o => o.Customer);
            query = query.Include(o => o.OrderItems)
                         .ThenInclude(oi => oi.OrderItemAddons)
                         .ThenInclude(oia => oia.Addon);

            // Use repository for paging
            var pagedOrders = await _unitOfWork.Orders.GetAllOrders(query, searchParams.PageNumber, searchParams.PageSize);

            // Select only requested fields
            var selectFields = searchParams.SelectFields;

            var result = pagedOrders.Select(order =>
            {
                var response = new OrderPlacingResponse
                {
                    Id = order.Id
                };

                // If no select fields specified, populate all fields
                if (selectFields == null || selectFields.Count == 0)
                {
                    response.OrderDate = order.OrderDate;
                    response.Status = order.Status.ToString();
                    response.TotalAmount = order.TotalAmount;
                    response.CustomerId = order.CustomerId ?? Guid.Empty;
                    response.OrderItems = order.OrderItems.Select(oi => new OrderItemResponse
                    {
                        Id = oi.Id,
                        ProductsWithVariant = oi.Variant?.Product != null ? new ProductWithVariantResponse
                        {
                            ProductId = oi.Variant.Product.Id,
                            ProductName = oi.Variant.Product.Name,
                            VariantId = oi.Variant.Id,
                            VariantSize = oi.Variant.Size.ToString(),
                            BasePrice = oi.Variant.BasePrice
                        } : null,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Notes = oi.Notes,
                        Temperature = oi.Temperature.ToString(),
                        Sweetness = oi.Sweetness.ToString(),
                        MilkType = oi.MilkType.ToString(),
                        Addons = oi.OrderItemAddons?.Select(oia => new CoffeeAddonResponse
                        {
                            Id = oia.AddonId,
                            Name = oia.Addon != null ? oia.Addon.Name : "",
                            Price = oia.Price
                        }).ToList() ?? new List<CoffeeAddonResponse>()
                    }).ToList();
                }
                else
                {
                    // Always include Id, populate only selected fields
                    foreach (var field in selectFields)
                    {
                        switch (field)
                        {
                            case OrderSearchParams.OrderSelectField.OrderDate:
                                response.OrderDate = order.OrderDate ?? null;
                                break;
                            case OrderSearchParams.OrderSelectField.Status:
                                response.Status = order.Status.ToString();
                                break;
                            case OrderSearchParams.OrderSelectField.TotalAmount:
                                response.TotalAmount = order.TotalAmount;
                                break;
                            case OrderSearchParams.OrderSelectField.Customer:
                                response.CustomerId = order.CustomerId ?? null;
                                break;
                            case OrderSearchParams.OrderSelectField.OrderItems:
                                response.OrderItems = order.OrderItems.Select(oi => new OrderItemResponse
                                {
                                    Id = oi.Id,
                                    ProductsWithVariant = oi.Variant?.Product != null ? new ProductWithVariantResponse
                                    {
                                        ProductId = oi.Variant.Product.Id,
                                        ProductName = oi.Variant.Product.Name,
                                        VariantId = oi.Variant.Id,
                                        VariantSize = oi.Variant.Size.ToString(),
                                        BasePrice = oi.Variant.BasePrice
                                    } : null,
                                    Quantity = oi.Quantity,
                                    UnitPrice = oi.UnitPrice,
                                    Notes = oi.Notes,
                                    Temperature = oi.Temperature.ToString(),
                                    Sweetness = oi.Sweetness.ToString(),
                                    MilkType = oi.MilkType.ToString(),
                                    Addons = oi.OrderItemAddons?.Select(oia => new CoffeeAddonResponse
                                    {
                                        Id = oia.AddonId,
                                        Name = oia.Addon != null ? oia.Addon.Name : "",
                                        Price = oia.Price
                                    }).ToList() ?? new List<CoffeeAddonResponse>()
                                }).ToList();
                                break;
                        }
                    }
                }

                return response;
            });

            return (result.ToList(), pagedOrders.MetaData);
        }

        public async Task<OrderPlacingResponse> GetOrderById(int orderId)
        {

            throw new NotImplementedException();

        }

        public async Task<(OrderPlacingResponse order, string paymentUrl)> PlaceOrder(OrderPlacingRequest request)
        {
            (string userId, string role) = _currentUserService.GetCurrentUser();
            var user = await _unitOfWork.Users.FindOneAsync(u => u.Id == Guid.Parse(userId)) ?? throw new NotFoundException("Not found user");
            if (role != Role.Customer.ToString() && role != Role.Admin.ToString())
            {
                throw new UnauthorizedAccessException("Only customers can place orders and admin");
            }

            if (request.OrderItems == null || request.OrderItems.Count == 0)
            {
                throw new InvalidOperationException("Order must have at least one item.");
            }

            try
            {
                await _unitOfWork.BeginTransaction();


                var order = new Order
                {
                    CustomerId = Guid.Parse(userId),
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.PROCESSING,
                    OrderCode = long.Parse(DateTime.UtcNow.ToString("yyyyMMddHHmmss"))
                };
                await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();  // Save để có order.Id

                var orderItems = new List<OrderDetail>();
                decimal totalAmount = 0;

                foreach (var itemReq in request.OrderItems)
                {
                    // Validate and get variant using repository
                    var variant = await _unitOfWork.CoffeeVariants.GetByIdAsync(
                        itemReq.VariantId,
                        q => q.Include(v => v.Product)
                    ) ?? throw new NotFoundException($"Variant with ID {itemReq.VariantId} not found or not active.");

                    if (!variant.IsActive)
                    {
                        throw new InvalidOperationException($"Variant with ID {itemReq.VariantId} is not active.");
                    }

                    if (variant.Product == null || !variant.Product.IsActive)
                    {
                        throw new InvalidOperationException($"Product for variant {itemReq.VariantId} is not active.");
                    }

                    // Validate addons if provided using repository
                    var addons = new List<CoffeeAddon>();
                    if (itemReq.AddonIds != null && itemReq.AddonIds.Any())
                    {
                        var allRequestedAddons = await _unitOfWork.CoffeeAddons.FindAsync(
                            a => itemReq.AddonIds.Contains(a.Id) && a.IsActive
                        );
                        addons = allRequestedAddons.ToList();

                        if (addons.Count != itemReq.AddonIds.Count)
                        {
                            var foundIds = addons.Select(a => a.Id).ToList();
                            var missingIds = itemReq.AddonIds.Except(foundIds).ToList();
                            throw new NotFoundException($"Some addons not found or not active: {string.Join(", ", missingIds)}");
                        }
                    }

                    // Calculate unit price (variant base price + addon prices)
                    var addonTotalPrice = addons.Sum(a => a.Price);
                    var unitPrice = variant.BasePrice + addonTotalPrice;

                    // Create OrderDetail
                    var orderItem = new OrderDetail
                    {
                        OrderId = order.Id,
                        VariantId = variant.Id,
                        Quantity = itemReq.Quantity,
                        UnitPrice = unitPrice,
                        Temperature = itemReq.Temperature,
                        Sweetness = itemReq.Sweetness,
                        MilkType = itemReq.MilkType,
                        OrderItemAddons = addons.Select(addon => new OrderItemAddon
                        {
                            AddonId = addon.Id,
                            Price = addon.Price
                        }).ToList()
                    };

                    orderItems.Add(orderItem);
                    totalAmount += unitPrice * itemReq.Quantity;
                }

                // Save order items
                await _unitOfWork.OrderDetails.AddRangeAsync(orderItems);
                await _unitOfWork.SaveChangesAsync();

                // Update order total amount
                order.TotalAmount = totalAmount;
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransaction();

                var paymentUrl = await _paymentService.CreatePaymentLink(order);

                // Reload order with all relations for response
                var orderWithDetails = await _unitOfWork.Orders.GetByIdAsync(order.Id,
                    q => q.Include(o => o.OrderItems)
                          .ThenInclude(oi => oi.Variant)
                          .ThenInclude(v => v.Product));

                if (orderWithDetails == null)
                {
                    throw new NotFoundException("Order not found after creation");
                }

                return (new OrderPlacingResponse
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    Status = order.Status.ToString(),
                    TotalAmount = order.TotalAmount,
                    CustomerId = order.CustomerId ?? Guid.Empty,

                    OrderItems = orderWithDetails.OrderItems.Select(oi => new OrderItemResponse
                    {
                        Id = oi.Id,
                        ProductsWithVariant = oi.Variant?.Product != null ? new ProductWithVariantResponse
                        {
                            ProductId = oi.Variant.Product.Id,
                            ProductName = oi.Variant.Product.Name,
                            VariantId = oi.Variant.Id,
                            VariantSize = oi.Variant.Size.ToString(),
                            BasePrice = oi.Variant.BasePrice
                        } : null,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Notes = oi.Notes,
                        Temperature = oi.Temperature.ToString(),
                        Sweetness = oi.Sweetness.ToString(),
                        MilkType = oi.MilkType.ToString(),
                        Addons = oi.OrderItemAddons?.Select(oia => new CoffeeAddonResponse
                        {
                            Id = oia.AddonId,
                            Name = oia.Addon != null ? oia.Addon.Name : "",
                            Price = oia.Price
                        }).ToList() ?? new List<CoffeeAddonResponse>()
                    }).ToList()
                }, paymentUrl);
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public async Task<OrderPlacingResponse> UpdateOrderStatus(OrderStatusUpdateRequest request)
        {
            (string userId, string role) = _currentUserService.GetCurrentUser();

            // Get order with all relations
            var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId,
                q => q.Include(o => o.OrderItems)
                      .ThenInclude(oi => oi.Variant)
                      .ThenInclude(v => v.Product)
                      .Include(o => o.OrderItems)
                      .ThenInclude(oi => oi.OrderItemAddons)
                      .ThenInclude(oia => oia.Addon)
                      .Include(o => o.Customer)
            ) ?? throw new NotFoundException("Order not found");

            var currentStatus = order.Status;
            var newStatus = request.NewStatus;

            try
            {
                await _unitOfWork.BeginTransaction();

                // Validate and update based on role
                if (role == Role.Barista.ToString() || role == Role.Admin.ToString())
                {
                    // Barista/Admin can update:
                    // 1. PROCESSING -> BREWING (bắt đầu pha chế)
                    // 2. BREWING -> READY (nếu PICKUP) hoặc DELIVERING (nếu DELIVERY) (hoàn tất pha chế)

                    if (currentStatus == OrderStatus.PROCESSING && newStatus == OrderStatus.BREWING)
                    {
                        // Bắt đầu pha chế
                        order.Status = newStatus;
                    }
                    else if (currentStatus == OrderStatus.BREWING)
                    {
                        // Hoàn tất pha chế
                        if (order.DeliveryType == DeliveryType.PICKUP && newStatus == OrderStatus.READY)
                        {
                            order.Status = newStatus;
                        }
                        else if (order.DeliveryType == DeliveryType.DELIVERY && newStatus == OrderStatus.DELIVERING)
                        {
                            order.Status = newStatus;
                        }
                        else
                        {
                            throw new InvalidOperationException(
                                $"Invalid status transition from BREWING. Expected READY for PICKUP or DELIVERING for DELIVERY, but got {newStatus}."
                            );
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Barista can only update status from PROCESSING to BREWING, or from BREWING to READY (PICKUP) or DELIVERING (DELIVERY). Current status: {currentStatus}, Requested: {newStatus}"
                        );
                    }
                }
                else if (role == Role.Customer.ToString())
                {
                    // Customer can update:
                    // DELIVERING -> COMPLETED (nhận hàng)
                    // PROCESSING -> CANCELLED (hủy đơn hàng)
                    // Verify customer owns this order
                    if (order.CustomerId != Guid.Parse(userId))
                    {
                        throw new UnauthorizedAccessException("You can only update your own orders.");
                    }

                    if (currentStatus == OrderStatus.DELIVERING && newStatus == OrderStatus.COMPLETED)
                    {

                        order.Status = newStatus;
                    }
                    else if (currentStatus == OrderStatus.PROCESSING && newStatus == OrderStatus.CANCELLED)
                    {
                        order.Status = newStatus;
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Customer can only update status from DELIVERING to COMPLETED, or from PROCESSING to CANCELLED. Current status: {currentStatus}, Requested: {newStatus}"
                        );
                    }
                }
                else
                {
                    throw new UnauthorizedAccessException("Only Barista, Admin, or Customer can update order status.");
                }

                // Update UpdatedAt timestamp
                order.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();

                // Return updated order as response
                return new OrderPlacingResponse
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    Status = order.Status.ToString(),
                    TotalAmount = order.TotalAmount,
                    CustomerId = order.CustomerId ?? Guid.Empty,
                    OrderItems = order.OrderItems.Select(oi => new OrderItemResponse
                    {
                        Id = oi.Id,
                        ProductsWithVariant = oi.Variant?.Product != null ? new ProductWithVariantResponse
                        {
                            ProductId = oi.Variant.Product.Id,
                            ProductName = oi.Variant.Product.Name,
                            VariantId = oi.Variant.Id,
                            VariantSize = oi.Variant.Size.ToString(),
                            BasePrice = oi.Variant.BasePrice
                        } : null,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Notes = oi.Notes,
                        Temperature = oi.Temperature.ToString(),
                        Sweetness = oi.Sweetness.ToString(),
                        MilkType = oi.MilkType.ToString(),
                        Addons = oi.OrderItemAddons?.Select(oia => new CoffeeAddonResponse
                        {
                            Id = oia.AddonId,
                            Name = oia.Addon != null ? oia.Addon.Name : "",
                            Price = oia.Price
                        }).ToList() ?? new List<CoffeeAddonResponse>()
                    }).ToList()
                };
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }
        }

        public async Task<bool> ProcessPayingOrder(OrderPayingRequest request)
        {
            var order = await _unitOfWork.Orders.FindOneAsync(o => o.OrderCode == request.OrderCode)
                ?? throw new NotFoundException("Order not found");
            try
            {
                await _unitOfWork.BeginTransaction();
                order.Status = OrderStatus.COMPLETED;
                order.PaymentStatus = PaymentStatus.PAID;
                _unitOfWork.Orders.Update(order);
                var payment = new Payment
                {
                    OrderId = order.Id,
                    PaymentDate = DateTime.UtcNow,
                    Amount = order.TotalAmount,
                    Method = PaymentMethod.OnlineBanking,
                    Status = PaymentStatus.PAID
                };
                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransaction();
                return true;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransaction();
                throw;
            }

        }


    }
}
