using TheLight_JoneBookShop_WebMVC.Models;

namespace TheLight_JoneBookShop_WebMVC.DTO
{
    public class ShopCartVM
    {
        public int IdshopCart { get; set; }

        public int Quantity { get; set; }

        public int Idbook { get; set; }
        public string Image { get; set; } = null!;
        public string BookName { get; set; } = null!;
        public decimal DiscountValue {  get; set; }
        public decimal Price { get; set; }

        public decimal Total
        {
            get
            {
                return Quantity * Price;
            }
        }
    }
}
