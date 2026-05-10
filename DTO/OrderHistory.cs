using TheLight_JoneBookShop_WebMVC.Models;

namespace TheLight_JoneBookShop_WebMVC.DTO
{
    public class OrderHistory
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? Tax { get; set; }
        public string? Voucher { get; set; }
        public string? Payment { get; set; }
        public string? Address { get; set; }
        public decimal Amount { get; set; }

    }
}
