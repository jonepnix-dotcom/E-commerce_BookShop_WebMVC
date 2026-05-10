using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using TheLight_JoneBookShop_WebMVC.Data;
using TheLight_JoneBookShop_WebMVC.DTO;
using TheLight_JoneBookShop_WebMVC.helper;
using TheLight_JoneBookShop_WebMVC.Models;

namespace TheLight_JoneBookShop_WebMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly DbjonebookshopContext _context;
        public BooksController(DbjonebookshopContext context)
        {
            _context = context;
        }
        [Route("cua-hang")]
        [HttpGet]
        public async Task<IActionResult> Shop(int page = 1, int pageSize = 6, int catId = 0, string sort = "")
        {
            
            // Query cơ bản
            var query = _context.Books
                .Include(b => b.IdbookTypeNavigation)
                .Include(b => b.Images.Where(img => img.Status == true))
                .Include(b => b.IddiscountNavigation)
                .AsQueryable();
            #region Lọc sản phẩm
            if (catId > 0)
            {
                query = query.Where(b => b.IdbookType == catId);
            }
            #endregion
            #region Sắp xếp sản phẩm
            switch (sort)
            {
                case "Newest":
                    query = query.OrderByDescending(x => x.PublicationDate);
                    break;
                case "Popular":
                    var popularBooks = _context.Orderdetails
                        .Where(od => od.IdbookOrderNavigation.Status == "Đã giao")
                        .GroupBy(od => od.Idbook)
                        .Select(g => new
                        {
                            BookId = g.Key,
                            TotalQuantity = g.Sum(od => od.Quantity)
                        });

                    query = query.Join(
                            popularBooks,
                            b => b.Idbook,
                            pb => pb.BookId,
                            (b, pb) => new { Book = b, pb.TotalQuantity }
                        )
                        .OrderByDescending(x => x.TotalQuantity)
                        .Select(x => x.Book);
                    break;
                case "Discount":
                    query = query
                        .Where(b => b.IddiscountNavigation.DiscountValue > 0
                                && b.IddiscountNavigation.Status == true
                                && b.IddiscountNavigation.StartDate <= DateTime.Today
                                && b.IddiscountNavigation.EndDate >= DateTime.Today)
                        .OrderByDescending(b => b.IddiscountNavigation.DiscountValue);
                    break;
            }
            #endregion
            #region Phân trang
            // Tính tổng số items
            var totalItems = await query.CountAsync();

            // Tính số trang
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Lấy dữ liệu Skip/Take
            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Đóng gói vào ViewModel
            var vm = new ShopVM
            {
                Books = books,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            // Kiểm tra nếu là request Ajax => trả về partial (nếu muốn)
            // (Hoặc bạn có thể luôn trả về View chính)
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                // Trả về partial hiển thị danh sách
                return PartialView("_P_ShopList", vm);
            }
            else
            {
                // Trả về View đầy đủ
                return View(vm);
            }
            #endregion
        }

        [HttpPost]
        public IActionResult Search(string? searchstring)
        {
            // Lấy danh sách sách kèm theo ảnh
            IQueryable<Book> bookSearch = _context.Books.Include(b => b.Images);
            if (!string.IsNullOrEmpty(searchstring))
            {
                bookSearch = bookSearch.Where(x => x.BookName.Contains(searchstring));
            }
            // Chuyển đổi sang List để gửi vào view
            var result = bookSearch.ToList();
            return PartialView("_P_SearchResult", result);
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("{slug}-{id}")]
        public async Task<IActionResult> Details(string slug, int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ErrorView", "Tool");
            }
            var data = ManagerTool.Get_ShopCart(HttpContext, _context);
            var item = data.FirstOrDefault(x => x.Idbook == id);
            if (item != null)
            {
                ViewBag.Quantity = item.Quantity;
            }
            else
            {
                ViewBag.Quantity = 1;
            }

            var book = await _context.Books
                        .Include(b => b.Images.Where(img => img.Status == true))
                        .Include(b=>b.IddiscountNavigation)
                        .Include(b => b.IdbookTypeNavigation)
                        .Include(b => b.BookAuthors)
                            .ThenInclude(ba => ba.IdauthorNavigation)
                        .FirstOrDefaultAsync(m => m.Idbook == id);
            if (book == null)
            {
                Redirect($"/error/view");
            }

            return View(book);
        }
    }
}
