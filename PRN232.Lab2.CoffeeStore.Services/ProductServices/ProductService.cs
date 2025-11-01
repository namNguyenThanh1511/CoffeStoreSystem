using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.Repositories;
using PRN232.Lab2.CoffeeStore.Repositories.Entities;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;
using PRN232.Lab2.CoffeeStore.Services.Exceptions;
using PRN232.Lab2.CoffeeStore.Services.Models;

namespace PRN232.Lab2.CoffeeStore.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // get all products
        public async Task<(List<ProductResponse>, MetaData metaData)> GetAllProductsAsync(ProductSearchParams searchParams)
        {
            var query = _unitOfWork.Products.Query();

            // Filtering
            if (!string.IsNullOrWhiteSpace(searchParams.Search))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchParams.Search) ||
                    (p.Description != null && p.Description.Contains(searchParams.Search)) ||
                    (p.Origin != null && p.Origin.Contains(searchParams.Search)) ||
                    p.RoastLevel.ToString().Contains(searchParams.Search) ||
                    p.BrewMethod.ToString().Contains(searchParams.Search)
                );
            }

            // Filter by IsActive
            if (searchParams.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == searchParams.IsActive.Value);
            }

            // Sorting
            string sortBy = string.IsNullOrWhiteSpace(searchParams.SortBy) ? "Name" : searchParams.SortBy;
            string sortOrder = string.IsNullOrWhiteSpace(searchParams.SortOrder) ? "asc" : searchParams.SortOrder.ToLower();

            switch (sortBy.ToLower())
            {
                case "name":
                    query = sortOrder == "asc" ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);
                    break;
                case "createdat":
                    query = sortOrder == "asc" ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt);
                    break;
                case "roastlevel":
                    query = sortOrder == "asc" ? query.OrderBy(p => p.RoastLevel) : query.OrderByDescending(p => p.RoastLevel);
                    break;
                case "brewmethod":
                    query = sortOrder == "asc" ? query.OrderBy(p => p.BrewMethod) : query.OrderByDescending(p => p.BrewMethod);
                    break;
                default:
                    query = sortOrder == "asc" ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);
                    break;
            }

            // Include navigation properties before passing to repository
            query = query.Include(p => p.Variants.Where(v => v.IsActive));

            // Use repository for paging
            var pagedProducts = await _unitOfWork.Products.GetAllProducts(query, searchParams.PageNumber, searchParams.PageSize);

            // Select only requested fields
            var selectFields = searchParams.SelectFields;

            var result = pagedProducts.Select(product =>
            {
                var response = new ProductResponse
                {
                    Id = product.Id
                };

                // If no select fields specified, populate all fields
                if (selectFields == null || selectFields.Count == 0)
                {
                    response.Name = product.Name;
                    response.Description = product.Description;
                    response.ImageUrl = product.ImageUrl;
                    response.Variants = product.Variants
                        .Where(v => v.IsActive)
                        .Select(v => new CoffeeVariantResponse
                        {
                            Id = v.Id,
                            Size = v.Size.ToString(),
                            BasePrice = v.BasePrice
                        })
                        .ToList();
                }
                else
                {
                    // Always include Id, populate only selected fields
                    foreach (var field in selectFields)
                    {
                        switch (field)
                        {
                            case ProductSearchParams.ProductSelectField.Name:
                                response.Name = product.Name;
                                break;
                            case ProductSearchParams.ProductSelectField.Description:
                                response.Description = product.Description;
                                break;
                            case ProductSearchParams.ProductSelectField.Origin:
                                // Note: Origin is not in ProductResponse, would need to add it if needed
                                break;
                            case ProductSearchParams.ProductSelectField.RoastLevel:
                                // Note: RoastLevel is not in ProductResponse, would need to add it if needed
                                break;
                            case ProductSearchParams.ProductSelectField.BrewMethod:
                                // Note: BrewMethod is not in ProductResponse, would need to add it if needed
                                break;
                            case ProductSearchParams.ProductSelectField.ImageUrl:
                                response.ImageUrl = product.ImageUrl;
                                break;
                            case ProductSearchParams.ProductSelectField.IsActive:
                                // Note: IsActive is not in ProductResponse, would need to add it if needed
                                break;
                            case ProductSearchParams.ProductSelectField.CreatedAt:
                                // Note: CreatedAt is not in ProductResponse, would need to add it if needed
                                break;
                            case ProductSearchParams.ProductSelectField.Variants:
                                response.Variants = product.Variants
                                    .Where(v => v.IsActive)
                                    .Select(v => new CoffeeVariantResponse
                                    {
                                        Id = v.Id,
                                        Size = v.Size.ToString(),
                                        BasePrice = v.BasePrice
                                    })
                                    .ToList();
                                break;
                        }
                    }
                }

                return response;
            });

            return (result.ToList(), pagedProducts.MetaData);
        }

        // get product by id
        public async Task<ProductDetailsResponse> GetProductByIdAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(
                id,
                q => q.Include(p => p.Variants)
            //.Include(p => p.ProductInMenus)
            //    .ThenInclude(pm => pm.Menu)
            );

            if (product == null)
                throw new NotFoundException("Product not found");

            return MapToProductDetailsResponse(product);
        }

        // add product
        public async Task<ProductResponse> AddProductAsync(ProductCreationRequest request)
        {
            Product product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Origin = request.Origin,
                RoastLevel = request.RoastLevel,
                BrewMethod = request.BrewMethod,
                ImageUrl = request.ImageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Variants = request.Variants.Select(v => new CoffeeVariant
                {
                    Size = v.Size,
                    BasePrice = v.BasePrice,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };
            product = await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            var result = await _unitOfWork.Products.GetByIdAsync(product.Id, q => q.Include(p => p.Variants));
            if (result == null)
                throw new NotFoundException("Product not found after creation");
            return MapToProductResponse(result);
        }

        // update product
        public async Task UpdateProductAsync(Guid id, ProductUpdationRequest request)
        {
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
            if (existingProduct == null)
                throw new NotFoundException("Product not found");

            if (request.Name != null)
                existingProduct.Name = request.Name;
            if (request.Description != null)
                existingProduct.Description = request.Description;
            if (request.Origin != null)
                existingProduct.Origin = request.Origin;
            if (request.RoastLevel.HasValue)
                existingProduct.RoastLevel = request.RoastLevel.Value;
            if (request.BrewMethod.HasValue)
                existingProduct.BrewMethod = request.BrewMethod.Value;
            if (request.ImageUrl != null)
                existingProduct.ImageUrl = request.ImageUrl;
            if (request.IsActive.HasValue)
                existingProduct.IsActive = request.IsActive.Value;

            existingProduct.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Products.Update(existingProduct);
            await _unitOfWork.SaveChangesAsync();
        }

        // delete product
        public async Task DeleteProductAsync(Guid id)
        {
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
            if (existingProduct == null)
                throw new NotFoundException("Product not found");

            _unitOfWork.Products.Remove(existingProduct);
            await _unitOfWork.SaveChangesAsync();
        }

        // mapping helpers
        private ProductResponse MapToProductResponse(Product product)
        {
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Variants = product.Variants
                    .Where(v => v.IsActive)
                    .Select(v => new CoffeeVariantResponse
                    {
                        Id = v.Id,
                        Size = v.Size.ToString(),
                        BasePrice = v.BasePrice
                    })
                    .ToList()
            };
        }

        private List<ProductResponse> MapToProductResponseList(IEnumerable<Product> products)
        {
            return products.Select(MapToProductResponse).ToList();
        }

        private ProductDetailsResponse MapToProductDetailsResponse(Product product)
        {
            return new ProductDetailsResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Origin = product.Origin,
                RoastLevel = product.RoastLevel.ToString(),
                BrewMethod = product.BrewMethod.ToString(),
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                Variants = product.Variants
                    .Where(v => v.IsActive)
                    .Select(v => new CoffeeVariantResponse
                    {
                        Id = v.Id,
                        Size = v.Size.ToString(),
                        BasePrice = v.BasePrice
                    })
                    .ToList()
            };
        }
    }
}
