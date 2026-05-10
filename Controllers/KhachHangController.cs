using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TheLight_JoneBookShop_WebMVC.Data;
using TheLight_JoneBookShop_WebMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using AutoMapper;

using TheLight_JoneBookShop_WebMVC.helper;
using TheLight_JoneBookShop_WebMVC.DTO;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Google;
using System;

namespace TheLight_JoneBookShop_WebMVC.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly DbjonebookshopContext _context;
        private readonly IMapper _mapper;
        public KhachHangController(DbjonebookshopContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        #region Login
        [Route("dang-nhap")]
        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
        [Route("dang-nhap")]
        [HttpPost]
        public async Task<IActionResult> Login(string? ReturnUrl, LoginVM model)
        {

            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var customer = _context.Clients.SingleOrDefault(x => x.UserName == model.UserName);
                if (customer == null)
                {
                    ModelState.AddModelError("UserName", "Sai tên tài khoản");
                }
                else
                {

                    byte[] password = HASHBYTES_SHA2_256.Hash(model.Password);
                    if (!customer.Status)
                    {
                        ModelState.AddModelError("UserName", "Tài khoản chưa xác thực Email");
                    }
                    else
                    {
                        if (!customer.Password.SequenceEqual(password))
                        {
                            ModelState.AddModelError("Password", "Sai mật khẩu");
                        }
                        else
                        {
                            var claims = new List<Claim>
                            {
                                new Claim("CustomerID",customer.UserName ?? ""),
                                new Claim(ClaimTypes.Role,"Customer")
                            };
                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                            await HttpContext.SignInAsync(
                                    CookieAuthenticationDefaults.AuthenticationScheme,
                                    claimsPrincipal,
                                    new AuthenticationProperties
                                    {
                                        IsPersistent = false
                                    }
                                    );
                            _context.Shopcarts
                                            .Where(s => s.Idsession == HttpContext.Session.Id)
                                            .ExecuteUpdate(setters => setters.SetProperty(s => s.Iduser, customer.Iduser));


                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Books");
                            }
                        }
                    }
                }
            }
            return View();
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
        #endregion
        #region Google Login

        // Action khởi tạo quá trình đăng nhập bằng Google
        [HttpGet]
        [Route("dang-nhap-tu-google")]
        public IActionResult GoogleLogin(string? returnUrl)
        {
            // Xây dựng RedirectUri và truyền kèm returnUrl
            var redirectUri = Url.Action(nameof(GoogleResponse), new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUri };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // Action xử lý callback từ Google
        [HttpGet]
        [Route("dang-nhap-google")]
        public async Task<IActionResult> GoogleResponse(string? ReturnUrl)
        {
            // Lấy kết quả xác thực từ Google (sử dụng GoogleDefaults.AuthenticationScheme)
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                // Nếu xác thực thất bại, chuyển hướng về trang đăng nhập
                return RedirectToAction(nameof(Login));
            }

            // Lấy thông tin claims trả về từ Google
            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction(nameof(Login));
            }

            // Kiểm tra trong CSDL xem đã có tài khoản nào với email này chưa
            var client = _context.Clients.SingleOrDefault(x => x.Email == email);
            if (client == null)
            {
                // Nếu chưa có, tạo tài khoản mới
                client = new Client
                {
                    Email = email,
                    Name = name!,
                    UserName = email, // Sử dụng email làm tên đăng nhập
                    Status = true,    // Đánh dấu là đã xác thực vì thông tin đến từ Google
                    RegistrationDate = DateTime.Now,      
                    Password = HASHBYTES_SHA2_256.Hash(ManagerTool.GenerateToken(20)),
                    Birthday = DateOnly.FromDateTime(DateTime.Now),
                    Phones = "0123456789",
                    Address = "HCMC"
                };

                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
            }

            // Tạo claims cho người dùng
            var userClaims = new List<Claim>
    {
        new Claim("CustomerID", client.UserName),
        new Claim(ClaimTypes.Role, "Customer")
    };

            var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Đăng nhập người dùng qua Cookie Authentication
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                new AuthenticationProperties { IsPersistent = false }
            );

            // Cập nhật giỏ hàng theo session hiện tại (nếu có)
            _context.Shopcarts
                .Where(s => s.Idsession == HttpContext.Session.Id)
                .ExecuteUpdate(setters => setters.SetProperty(s => s.Iduser, client.Iduser));

            // Chuyển hướng đến ReturnUrl nếu hợp lệ, ngược lại về trang Books
            if (Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }
            return RedirectToAction("Index", "Books");
        }


        #endregion
        #region Profile
        [AcceptVerbs("GET", "POST")]
        public IActionResult CheckEmailOrder(string email)
        {
            // 1. Lấy username từ Claims
            var username = User.FindFirst("CustomerID")?.Value;
            if (string.IsNullOrEmpty(username))
            {
                // Chưa đăng nhập hoặc claims rỗng => Tùy logic, 
                // ví dụ: return Json(true) => bỏ qua check
                return Json(true);
            }

            // 2. Tìm user trong DB
            var currentUser = _context.Clients
                .SingleOrDefault(u => u.UserName == username);

            if (currentUser == null)
            {
                // Không tìm thấy user => tùy xử lý, 
                // ví dụ return Json(true) => ko check
                return Json(true);
            }

            // 3. Nếu email nhập vào == email cũ => cho phép
            if (email == currentUser.Email)
            {
                return Json(true);
            }

            // 4. Kiểm tra email có tồn tại ở người dùng khác không
            var otherUser = _context.Clients
                .SingleOrDefault(u => u.Email == email);

            if (otherUser != null)
            {
                // Đã có user khác dùng email này => báo lỗi
                return Json($"Email {email} đã tồn tại.");
            }

            // 5. Không trùng => hợp lệ
            return Json(true);
        }
        [Route("thong-tin-tai-khoan")]
        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            // 1. Lấy username từ Claims
            var username = User.FindFirst("CustomerID")?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "KhachHang");
            }

            // 2. Tìm user trong DB
            var user = _context.Clients.SingleOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return RedirectToAction("ErrorView", "Tool");
            }
            var vm = new ProfileVM
            {
                Name = user.Name,
                Address = user.Address,
                Email = user.Email,
                Phones = user.Phones,
                Birthday = user.Birthday
            };
            var pr = new ParentVM
            {
                ProfileInfo = vm
            };
            return View(pr);
        }
        [Authorize]
        [Route("thong-tin-tai-khoan")]
        [HttpPost]
        public IActionResult Profile(ProfileVM model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu sai ràng buộc => quay lại view kèm lỗi
                return View(model);
            }

            // 1. Lấy username từ claim
            var username = User.FindFirst("CustomerID")?.Value;
            var user = _context.Clients.SingleOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return RedirectToAction("ErrorView", "Tool");
            }

            // 2. Map RegisterVM -> Client
            if (model.Name != null)
                user.Name = model.Name;
            user.Address = model.Address;
            user.Email = model.Email;
            user.Phones = model.Phones;
            // Convert DateOnly? -> DateTime?
            user.Birthday = model.Birthday;

            // 3. Lưu DB
            _context.SaveChanges();

            // 4. Quay lại Profile (hoặc trang khác)
            return RedirectToAction("Profile");
        }
        [Authorize]
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                // Trả về view Profile, hiển thị form đổi mật khẩu
                return View(model);
            }

            // 1. Lấy user
            var username = User.FindFirst("CustomerID")?.Value;
            var user = _context.Clients.SingleOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return RedirectToAction("ErrorView", "Tool");
            }

            if (model.Password != null)
                user.Password = HASHBYTES_SHA2_256.Hash(model.Password);
            _context.SaveChanges();

            return RedirectToAction("Profile");
        }
        #endregion
        #region Register
        [AcceptVerbs("GET", "POST")]
        public IActionResult CheckUserName(string userName)
        {
            if (_context.Clients.Any(u => u.UserName == userName))
            {
                return Json($"Tên tài khoản {userName} đã tồn tại.");
            }
            return Json(true);
        }
        [AcceptVerbs("GET", "POST")]
        public IActionResult CheckEmail(string email)
        {
            if (_context.Clients.Any(c => c.Email == email))
            {
                return Json($"Email {email} đã tồn tại.");
            }
            return Json(true);
        }
        [HttpGet]
        [Route("dang-ky")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [Route("dang-ky")]
        public async Task<IActionResult> Register(RegisterVM model)
        {

            if (ModelState.IsValid)
            {
                if (_context.Clients.Any(c => c.Email == model.Email))
                {
                    ModelState.AddModelError("Email", $"Email {model.Email} đã tồn tại.");
                    return View(model);
                }
                if (!_context.Clients.Any(u => u.UserName == model.UserName))
                {
                    var client = _mapper.Map<Client>(model);
                    if (model.Password != null)
                        client.Password = HASHBYTES_SHA2_256.Hash(model.Password);
                    client.Status = false;
                    client.RegistrationDate = DateTime.Now;
                    _context.Add(client);
                    await _context.SaveChangesAsync();
                    //Send verify mail
                    EmailVerification email = new EmailVerification
                    {
                        Token = ManagerTool.GenerateToken(10),
                        UserId = client.Iduser,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(3)
                    };
                    _context.Add(email);
                    ManagerTool.XacMinhTaiKhoan(client.Email ?? "", email.Token);

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Login", "KhachHang");
                }
                else
                {
                    ModelState.AddModelError("UserName", "Tên tài khoản đã tồn tại.");
                    return View(model);
                }
            }
            return RedirectToAction("ErrorView", "Tool");
        }
        #endregion
        #region Verify
        [Route("xac-thuc")]
        public IActionResult XacThuc(string token)
        {
            if (token == null)
                return RedirectToAction("ErrorView", "Tool");
            var Token = _context.EmailVerifications
                .Include(x => x.User)
                .FirstOrDefault(t => t.Token == token);
            if (Token == null)
                return RedirectToAction("ErrorView", "Tool");
            else
            {
                Token.User.Status = true;
                _context.SaveChanges();
            }
            return View();
        }
        #endregion
    }
}
