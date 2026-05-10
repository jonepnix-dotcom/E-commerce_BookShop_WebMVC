namespace TheLight_JoneBookShop_WebMVC.DTO
{
    public class OrderDetailVM
    {
        public string BookName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}
