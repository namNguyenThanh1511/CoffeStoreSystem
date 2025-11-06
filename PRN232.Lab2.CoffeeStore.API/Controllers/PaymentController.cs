using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Newtonsoft.Json;
using PRN232.Lab2.CoffeeStore.Services.Models.Order;
using PRN232.Lab2.CoffeeStore.Services.Models.Payment;
using PRN232.Lab2.CoffeeStore.Services.OrderService;
using PRN232.Lab2.CoffeeStore.Services.PaymentService;

namespace PRN232.Lab2.CoffeeStore.API.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly ILogger<PaymentController> _logger;


        public PaymentController(IPaymentService paymentService, IOrderService orderService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _orderService = orderService;
            _logger = logger;
        }



        [HttpPost("webhook/payos")]
        public async Task<IActionResult> HandleWebhook()
        {
            try
            {
                Request.EnableBuffering();
                using var reader = new StreamReader(Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                Request.Body.Position = 0;

                _logger.LogInformation("Received PayOS webhook body: {Body}", body);

                if (string.IsNullOrWhiteSpace(body))
                    return BadRequest("Empty body");

                // ✅ Deserialize chính xác theo PayOS SDK
                var webhookType = JsonConvert.DeserializeObject<WebhookType>(body);
                if (webhookType == null)
                    return BadRequest("Invalid JSON format");

                // ✅ Gọi verify đúng kiểu dữ liệu
                var webhookData = _paymentService.VerifyPaymentWebhookData(webhookType);



                if (webhookData.code == "00")
                {
                    string description = webhookData.description;

                    if (description.Contains("PAYORDER"))
                    {
                        long orderCode = webhookData.orderCode;
                        _logger.LogInformation("✅ Payment success: OrderCode={OrderCode}, Amount={Amount}",
                            webhookData.orderCode, webhookData.amount);
                        await _orderService.ProcessPayingOrder(new OrderPayingRequest
                        {
                            OrderCode = orderCode
                        });
                    }
                    else
                    {
                        _logger.LogWarning("Description format unexpected: {Description}", description);
                    }
                    return Ok();
                }
                else
                {
                    _logger.LogWarning("❌ Invalid webhook signature or failed verification");
                    return BadRequest("Verification failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing PayOS webhook");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("payos/return")]
        public async Task<IActionResult> Return([FromQuery] PayOsReturnRequest payos)
        {
            try
            {
                // 1️⃣ Kiểm tra dữ liệu bắt buộc
                if (string.IsNullOrEmpty(payos.OrderCode))
                    return BadRequest("Thiếu mã đơn hàng.");

                if (string.IsNullOrEmpty(payos.Status))
                    return BadRequest("Thiếu trạng thái thanh toán.");

                // 2️⃣ Log request (để kiểm tra debug hoặc audit)
                _logger.LogInformation("PayOS Return Callback: {@PayOs}", payos);

                // 3️⃣ Xác minh transaction từ PayOS (tùy SDK)
                //    Bạn có thể gọi lại PayOS API bằng orderCode để confirm thật sự
                //    Ví dụ:
                //    var verified = await _payOsService.VerifyPaymentAsync(payos.OrderCode);
                //    if (!verified.Success) return BadRequest("Xác thực thanh toán thất bại.");

                // 4️⃣ Nếu status hợp lệ => xử lý cập nhật đơn hàng
                if (payos.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase) && payos.Code == "00")
                {
                    await _orderService.ProcessPayingOrder(new OrderPayingRequest
                    {
                        OrderCode = long.Parse(payos.OrderCode),
                    });

                    _logger.LogInformation($"Đơn hàng {payos.OrderCode} đã thanh toán thành công.");
                }
                else
                {
                    _logger.LogWarning($"Thanh toán thất bại cho đơn hàng {payos.OrderCode} - Status: {payos.Status}");
                }

                // 5️⃣ Redirect về app
                var deepLink = $"mycoffeeapp://payos/return?" +
                               $"status={payos.Status}&" +
                               $"orderCode={payos.OrderCode}&" +
                               $"cancel={payos.Cancel.ToString().ToLower()}";

                return Content($@"<html>
                                        <head>
                                         <meta http-equiv='refresh' content='0;url={deepLink}' />
                                        </head>
                                        <body>
                                            <h2>Thanh toán thành công 🎉</h2>
                                            <p>Đang quay lại ứng dụng...</p>
                                        <script>window.location.href='{deepLink}';</script>

                                        </body>
                                  </html>",
                                  "text/html;charset=utf-8");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xử lý PayOS callback");
                return StatusCode(500, "Lỗi xử lý callback.");
            }
        }


        [HttpGet("payos/cancel")]
        public IActionResult Cancel([FromQuery] string orderCode)
        {
            var deepLink = $"mycoffeeapp://payos/cancel?orderCode={orderCode}";
            return Redirect(deepLink);
        }



    }
}
