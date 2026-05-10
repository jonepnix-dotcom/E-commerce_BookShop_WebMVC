using TheLight_JoneBookShop_WebMVC.Models;

namespace TheLight_JoneBookShop_WebMVC.DTO
{
    public class ShopVM
    {
        public required IEnumerable<Book> Books { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
