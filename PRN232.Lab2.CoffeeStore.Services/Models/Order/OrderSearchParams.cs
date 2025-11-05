using System.ComponentModel;
using System.Text.Json.Serialization;
namespace PRN232.Lab2.CoffeeStore.Services.Models.Order
{
    public class OrderSearchParams : RequestParams
    {
        public string Search { get; set; }
        public string SortBy { get; set; } = "OrderDate"; // Default sort by OrderDate
        public string SortOrder { get; set; } = "desc"; // Default sort order descending
        public string Field { get; set; } = "All"; // Default filter by all
        //filter by Status
        public List<string> Statuses { get; set; } = new List<string>();
        //filter by DeliveryType
        public List<string> DeliveryTypes { get; set; } = new List<string>();
        //filter by PaymentStatus
        public List<string> PaymentStatuses { get; set; } = new List<string>();

        public List<OrderSelectField> SelectFields { get; set; } = new List<OrderSelectField>();

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum OrderSelectField
        {
            [Description("OrderDate")]
            OrderDate,
            [Description("TotalAmount")]
            TotalAmount,
            [Description("Status")]
            Status,
            [Description("Customer")]
            Customer,
            [Description("OrderItems")]
            OrderItems,
        }

    }
}
