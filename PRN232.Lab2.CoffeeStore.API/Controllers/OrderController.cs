using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.API.Models;
using PRN232.Lab2.CoffeeStore.Services.Models.Order;
using PRN232.Lab2.CoffeeStore.Services.OrderService;
using PRN232.Lab2.CoffeeStore.Services.PaymentService;

namespace PRN232.Lab2.CoffeeStore.API.Controllers
{
    [Route("api/orders")]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        public OrderController(IOrderService orderService, IPaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        }

        [HttpGet("admin-barista")]
        [Authorize(Roles = "Admin,Barista")]
        public async Task<ActionResult<ApiResponse<List<OrderPlacingResponse>>>> GetAllOrders([FromQuery] OrderSearchParams searchParams)
        {
            var (orders, metaData) = await _orderService.GetAllOrders(searchParams);
            // Add pagination metadata to response header
            Response.Headers.Append("X-Pagination-CurrentPage", metaData.CurrentPage.ToString());
            Response.Headers.Append("X-Pagination-TotalPages", metaData.TotalPages.ToString());
            Response.Headers.Append("X-Pagination-PageSize", metaData.PageSize.ToString());
            Response.Headers.Append("X-Pagination-TotalCount", metaData.TotalCount.ToString());
            Response.Headers.Append("X-Pagination-HasPrevious", metaData.HasPrevious.ToString());
            Response.Headers.Append("X-Pagination-HasNext", metaData.HasNext.ToString());

            return Ok(orders);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<OrderPlacingResponse>>>> GetAllOrdersByCurrentUser([FromQuery] OrderSearchParams searchParams)
        {
            var (orders, metaData) = await _orderService.GetOrdersByCurrentUser(searchParams);
            // Add pagination metadata to response header
            Response.Headers.Append("X-Pagination-CurrentPage", metaData.CurrentPage.ToString());
            Response.Headers.Append("X-Pagination-TotalPages", metaData.TotalPages.ToString());
            Response.Headers.Append("X-Pagination-PageSize", metaData.PageSize.ToString());
            Response.Headers.Append("X-Pagination-TotalCount", metaData.TotalCount.ToString());
            Response.Headers.Append("X-Pagination-HasPrevious", metaData.HasPrevious.ToString());
            Response.Headers.Append("X-Pagination-HasNext", metaData.HasNext.ToString());

            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<OrderPlacingResponse>>> GetOrderById([FromRoute] int orderId)
        {
            var order = await _orderService.GetOrderById(orderId);
            return Ok(order);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<OrderPlacingResponse>>> CreateOrder([FromBody] OrderPlacingRequest request)
        {
            var result = await _orderService.PlaceOrder(request);
            Response.Headers.Append("X-Foraward-Payment", result.paymentUrl);
            return Created(result.order, "Đặt hàng thành công");
        }

        [HttpPost("paying")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> ProcessPayingOrder([FromBody] OrderPayingRequest request)
        {
            var success = await _orderService.ProcessPayingOrder(request);
            if (success)
            {
                return Ok("Xử lý đơn hàng thành công");
            }
            else
            {
                return BadRequest("Xử lý đơn hàng thất bại");
            }
        }

        [HttpPut("status")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<OrderPlacingResponse>>> UpdateOrderStatus([FromBody] OrderStatusUpdateRequest request)
        {
            var updatedOrder = await _orderService.UpdateOrderStatus(request);
            return Ok(updatedOrder, "Cập nhật trạng thái đơn hàng thành công");
        }

    }
}
