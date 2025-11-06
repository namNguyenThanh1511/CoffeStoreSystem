namespace PRN232.Lab2.CoffeeStore.Services.Models.Payment
{
    public class PayOsReturnRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public bool Cancel { get; set; }
        public string Status { get; set; } = string.Empty;
        public string OrderCode { get; set; } = string.Empty;
    }

}
