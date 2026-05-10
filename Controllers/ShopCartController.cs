using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net;
using TheLight_JoneBookShop_WebMVC.Data;
using TheLight_JoneBookShop_WebMVC.DTO;
using TheLight_JoneBookShop_WebMVC.helper;
using TheLight_JoneBookShop_WebMVC.Models;
using TheLight_JoneBookShop_WebMVC.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TheLight_JoneBookShop_WebMVC.Controllers
{
    public class ShopCartController : Controller
    {
        private readonly DbjonebookshopContext _context;
        private readonly IMapper _mapper;
        private readonly ShippingService _shippingService;
        private readonly IVnPayService _vnPayService;

        public ShopCartController(DbjonebookshopContext context, IMapper mapper, ShippingService shippingService, IVnPayService vnPayService)
        {
            _context = context;
            _mapper = mapper;
            _shippingService = shippingService;
            _vnPayService = vnPayService;
        }
        #region Giỏ Hàng
        [Route("gio-hang")]
        public IActionResult Index()
        {
            var data = ManagerTool.Get_ShopCart(HttpContext, _context);
            var shopcarts = _mapper.Map<List<ShopCartVM>>(data);
            return View(shopcarts);
        }

        public async Task<IActionResult> AddToCard(int id, int quantity = 1)
        {
            var item = ManagerTool.Get_ShopCart(HttpContext, _context).SingleOrDefault(x => x.Idbook == id);
            if (item == null)
            {
                var book = _context.Books.SingleOrDefault(x => x.Idbook == id);
                if (book == null)
                {
                    return RedirectToAction("ErrorView", "Tool");

                }
                var username = User.FindFirst("CustomerID")?.Value;
                var user = _context.Clients.SingleOrDefault(u => u.UserName == username);
                if (user != null)
                {
                    item = new Shopcart
                    {
                        Idbook = book.Idbook,
                        Idsession = HttpContext.Session.Id,
                        Iduser = user.Iduser,
                        ExpirationDate = DateTime.UtcNow.AddMinutes(30),
                        Quantity = quantity
                    };
                }
                else
                {
                    item = new Shopcart
                    {
                        Idbook = book.Idbook,
                        Idsession = HttpContext.Session.Id,
                        Iduser = 1,
                        ExpirationDate = DateTime.UtcNow.AddMinutes(30),
                        Quantity = quantity
                    };
                }
                _context.Add(item);
            }
            else
            {
                item.Quantity = quantity;
                _context.Update(item);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> PlusMinus(int id, int quantity)
        {
            var shopcart = ManagerTool.Get_ShopCart(HttpContext, _context);
            var item = shopcart.SingleOrDefault(x => x.Idbook == id);
            if (item == null)
            {
                return RedirectToAction("ErrorView", "Tool");
            }
            else
            {
                item.Quantity += quantity;
                if (item.Quantity < 1)
                {
                    _context.Remove(item);
                }
                else
                {
                    _context.Update(item);
                }
            }
            await _context.SaveChangesAsync();
            // Lấy lại danh sách giỏ hàng từ database sau khi cập nhật
            shopcart = ManagerTool.Get_ShopCart(HttpContext, _context);
            var shopcartVM = _mapper.Map<List<ShopCartVM>>(shopcart);
            var itemprice = shopcartVM.SingleOrDefault(x => x.Idbook == id);

            return Json(new
            {
                success = true,
                total = (itemprice != null ? itemprice.Total : 0).ToString("#,##0"),
                totalprice = shopcartVM.Sum(x => x.Total),
                quantitybook = item.Quantity,
            });
        }
        #endregion
        #region Thanh Toán
        [Authorize]
        [Route("thanh-toan")]
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var shopcart = ManagerTool.Get_ShopCart(HttpContext, _context);
            var shopcartVM = _mapper.Map<List<ShopCartVM>>(shopcart);
            decimal basePrice = shopcartVM.Sum(x => x.Total);
            ViewBag.basePrice = basePrice.ToString("#,##0") + " VND";

            var tax = _context.Taxes.FirstOrDefault(x => x.Status == true);
            decimal taxvalue = tax!.Tax1 * shopcartVM.Sum(x => x.Total) / 100;
            ViewBag.tax = tax!.Description + " | " + tax!.Tax1.ToString("N0") + "%" + " ~ " + taxvalue.ToString("#,##0") + " VND";

            var username = User.FindFirst("CustomerID")?.Value;
            var currentUser = _context.Clients
                .SingleOrDefault(u => u.UserName == username);
            int totalBooks = shopcartVM.Sum(x => x.Quantity);
            string recipientAddress = currentUser!.Address!;
            decimal fee = await _shippingService.CalculateShippingFeeAsync(recipientAddress, (int)(shopcartVM.Sum(x => x.Total)), totalBooks * 2, 15, totalBooks * 400, 13);
            ViewBag.shipfee = fee.ToString("#,##0") + " VND";

            decimal totalPayment = (shopcartVM.Sum(x => x.Total)) + taxvalue + fee - 0;
            ViewBag.totalPayment = totalPayment.ToString("#,##0") + " VND";


            // Xóa tất cả Bookorder có Status là "Processing"
            var ordersToDelete = await _context.Bookorders
                .Where(o => o.Iduser == currentUser.Iduser && o.Status.ToLower() == "processing")
                .ToListAsync();

            _context.Bookorders.RemoveRange(ordersToDelete);
            await _context.SaveChangesAsync();

            // Tạo đơn hàng mới
            var order = new Bookorder
            {
                Iduser = currentUser.Iduser,
                OrderDate = DateTime.Now,
                TotalPrice = totalPayment,
                Status = "Processing",
                Iddiscount = 1,
                Idvoucher = 1,
                Idtax = tax.Idtax,
                Idpayment = 1,
                Address = currentUser.Address!,
                DeliveryDate = null
            };

            _context.Add(order);
            await _context.SaveChangesAsync();


            CheckoutVM ck = new CheckoutVM
            {
                UserId = currentUser.Iduser,
                Amount = (int)totalPayment,
                Name = currentUser.Name,
                Email = currentUser.Email,
                Address = currentUser.Address
            };
            return View(ck);
        }

        [Authorize]
        [Route("thanh-toan")]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM checkoutVM)
        {
            if (ModelState.IsValid)
            {
                var VnPayModel = new VnPaymentRequestModel
                {
                    Amount = checkoutVM.Amount,
                    CreatedDate = DateTime.Now,
                    FullName = checkoutVM.Name!,
                    OrderId = new Random().Next(1000, 100000)
                };
                return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, VnPayModel));
            }
            return RedirectToAction("Index", "ShopCart");
        }
        [Authorize]
        public async Task<IActionResult> PaymentBack()
        {
            var response = _vnPayService.PaymentExcute(Request.Query);
            if (response == null || response.VnPayResponseCode != "00")
            {
                return RedirectToAction("ErrorView", "Tool");
            }
            //Xử lý order

            var username = User.FindFirst("CustomerID")?.Value;
            var currentUser = _context.Clients
                .SingleOrDefault(u => u.UserName == username);
            var order = await _context.Bookorders
                            .FirstOrDefaultAsync(o => o.Iduser == currentUser!.Iduser && o.Status == "Processing");
            if (order != null)
            {
                order.Status = "Đang vận chuyển";
                order.OrderDate = DateTime.Now;

                // Chỉ cập nhật những trường đã thay đổi
                _context.Entry(order).Property(o => o.Status).IsModified = true;
                _context.Entry(order).Property(o => o.OrderDate).IsModified = true;

                await _context.SaveChangesAsync();
            }
            var voucher = _context.Vouchers.FirstOrDefault(v => v.Id == order!.Idvoucher && v.Id != 1);
            if (voucher != null)
            {
                voucher.UsedCount += 1;
                voucher.UsageLimit -= 1;
                // Chỉ cập nhật những trường đã thay đổi
                _context.Entry(voucher).Property(o => o.UsedCount).IsModified = true;
                _context.Entry(voucher).Property(o => o.UsageLimit).IsModified = true;
                await _context.SaveChangesAsync();
            }

            var data = ManagerTool.Get_ShopCart(HttpContext, _context);
            var shopcarts = _mapper.Map<List<ShopCartVM>>(data);
            var orderdetail = _mapper.Map<List<Orderdetail>>(shopcarts);
            foreach (var item in orderdetail)
            {
                item.IdbookOrder = order!.IdbookOrder;
            }
            _context.AddRange(orderdetail);

            var delShopcart = await _context.Shopcarts.Where(s => s.Iduser == currentUser!.Iduser).ToListAsync();
            _context.RemoveRange(delShopcart);
            await _context.SaveChangesAsync();

            return RedirectToAction("PaymentSuccess", "Tool");
        }
        public async Task<IActionResult> RefreshBill(string address = "", string VoucherCode = "")
        {
            decimal vouchervalue = 0;
            string shipfee = "Không tìm thấy địa chỉ";
            decimal totalPayment = 0;
            var shopcart = ManagerTool.Get_ShopCart(HttpContext, _context);
            var shopcartVM = _mapper.Map<List<ShopCartVM>>(shopcart);
            var voucher = _context.Vouchers.FirstOrDefault(v => v.Code == VoucherCode
                && v.Status == true
                && v.EndDate >= DateTime.Now
                && v.StartDate <= DateTime.Now
                && v.UsageLimit > 0);
            string vouchertext = "0 VND";
            string mess = "Sử dụng thành công!";

            var username = User.FindFirst("CustomerID")?.Value;
            var currentUser = _context.Clients
                .SingleOrDefault(u => u.UserName == username);
            var order = await _context.Bookorders
                            .FirstOrDefaultAsync(o => o.Iduser == currentUser!.Iduser && o.Status == "Processing");

            if (voucher == null)
            {
                voucher = _context.Vouchers.FirstOrDefault(v=>v.Id == 1);
                if (order != null)
                {
                    order.Idvoucher = 1;

                    // Chỉ cập nhật những trường đã thay đổi
                    _context.Entry(order).Property(o => o.Idvoucher).IsModified = true;

                    await _context.SaveChangesAsync();
                }
                mess = "Mã giảm giá không chính xác hoặc đã hết hạn!";
            }
            else
            {
                decimal totalPrice = shopcartVM.Sum(x => x.Total);
                if (voucher.MinOrderValue > totalPrice)
                {
                    decimal require = (decimal)(voucher.MinOrderValue - totalPrice);
                    if (order != null)
                    {
                        order.Idvoucher = 1;

                        // Chỉ cập nhật những trường đã thay đổi
                        _context.Entry(order).Property(o => o.Idvoucher).IsModified = true;

                        await _context.SaveChangesAsync();
                    }
                    mess = "Mua thêm " + require.ToString("#,##0") + " VND để sử dụng mã giảm giá!";
                }
                else
                {
                    if (order != null)
                    {
                        order.Idvoucher = voucher.Id;

                        // Chỉ cập nhật những trường đã thay đổi
                        _context.Entry(order).Property(o => o.Idvoucher).IsModified = true;

                        await _context.SaveChangesAsync();
                    }
                    if (voucher != null)
                    {
                        if (voucher!.DiscountType == "%")
                        {
                            vouchervalue = voucher!.DiscountValue * shopcartVM.Sum(x => x.Total) / 100;
                            vouchertext = voucher.DiscountValue + " %" + " ~ " + vouchervalue.ToString("#,##0") + " VND";
                        }
                        else
                        {
                            vouchervalue = (int)voucher!.DiscountValue;
                            vouchertext = vouchervalue.ToString("#,##0") + " VND";
                        }

                    }
                }

            }
            try
            {
                // Tính tổng số lượng sách trong giỏ
                int totalBooks = shopcartVM.Sum(x => x.Quantity);

                string recipientAddress = address;
                decimal fee = await _shippingService.CalculateShippingFeeAsync(recipientAddress, (int)(shopcartVM.Sum(x => x.Total)), totalBooks * 2, 15, totalBooks * 400, 13);
                shipfee = fee.ToString("#,##0") + " VND";



                
                var tax = _context.Taxes.FirstOrDefault(x => x.Status == true);
                decimal taxvalue = tax!.Tax1 * shopcartVM.Sum(x => x.Total) / 100;
                totalPayment = (shopcartVM.Sum(x => x.Total)) + taxvalue + fee - vouchervalue;



            }
            catch
            {
                return Json(new
                {
                    mess = mess,
                    success = true,
                    shipfee = shipfee,
                    totalPayment = 0,
                    vouchervalue = vouchertext
                });
            }


            if (order != null)
            {
                order.TotalPrice = totalPayment;
                order.Idvoucher = voucher!.Id;
                order.Address = address;

                // Chỉ cập nhật những trường đã thay đổi
                _context.Entry(order).Property(o => o.TotalPrice).IsModified = true;
                _context.Entry(order).Property(o => o.Idvoucher).IsModified = true;
                _context.Entry(order).Property(o => o.Address).IsModified = true;

                await _context.SaveChangesAsync();
            }
            return Json(new
            {
                mess = mess,
                success = true,
                shipfee = shipfee,
                vouchervalue = vouchertext,
                totalPayment = totalPayment
            });
        }
        #endregion
        #region Lịch Sử Thanh Toán
        [Route("lich-su-thanh-toan")]
        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            var username = User.FindFirst("CustomerID")?.Value;
            var currentUser = _context.Clients
                .SingleOrDefault(u => u.UserName == username);
            var currentOrder = await _context.Bookorders.Where(b => b.Iduser == currentUser!.Iduser && b.Status != "Processing")
                .Include(b => b.IdvoucherNavigation)
                .Include(b => b.IdtaxNavigation)
                .Include(b => b.IdpaymentNavigation)
                .Include(b => b.Orderdetails)
                    .ThenInclude(d => d.IdbookNavigation).ToListAsync();
            var OrderH = _mapper.Map<List<OrderHistory>>(currentOrder);
            return View(OrderH);
        }
        [Authorize]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var orderDetails = await _context.Orderdetails
                .Where(od => od.IdbookOrder == orderId)
                .Select(od => new OrderDetailVM
                {
                    BookName = od.IdbookNavigation.BookName,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToListAsync();

            return Json(orderDetails);
        }
        public IActionResult InHoaDon(int orderId)
        {
            var email = "jonepnix@gmail.com";
            var username = User.FindFirst("CustomerID")?.Value;
            var currentUser = _context.Clients
                .SingleOrDefault(u => u.UserName == username);
            if (currentUser != null)
            {
                email = currentUser.Email;
            }
            if (ManagerTool.XuatHoaDonDienTu(email!, orderId, _context))
            {
                return Json(new
                {
                    success = true,
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                });
            }
        }

        #endregion
    }
}
