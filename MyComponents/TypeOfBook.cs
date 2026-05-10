using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheLight_JoneBookShop_WebMVC.Data;
using TheLight_JoneBookShop_WebMVC.DTO;

namespace TheLight_JoneBookShop_WebMVC.MyComponents
{
    public class TypeOfBook : ViewComponent
    {
        private readonly DbjonebookshopContext _context;
        public TypeOfBook(DbjonebookshopContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            var data = _context.Booktypes
                .Select(loai => new TypeOfBook_ViewComponents
                {
                    id = loai.IdbookType,
                    name = loai.BookTypeName,
                    quantity = _context.Books.Where(x => x.IdbookType == loai.IdbookType).Count()
                })
                .Where(type => type.quantity > 0)
                .ToList();
            return View(data);
        }
    }
    
}
