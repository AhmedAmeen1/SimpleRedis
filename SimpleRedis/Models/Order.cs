namespace SimpleRedis.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Product { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
