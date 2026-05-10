using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Text;
using TheLight_JoneBookShop_WebMVC.Data;
using TheLight_JoneBookShop_WebMVC.Models;
using System.Net.Mail;
using System.Net;
using System;
using System.Security.Cryptography;
using TheLight_JoneBookShop_WebMVC.DTO;

namespace TheLight_JoneBookShop_WebMVC.helper
{
    public static class ManagerTool
    {
        private static IConfiguration? _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private static readonly string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public static string GenerateToken(int length = 10)
        {
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[RandomNumberGenerator.GetInt32(s.Length)])
                .ToArray());
        }
        public static List<Shopcart> Get_ShopCart(HttpContext httpContext, DbjonebookshopContext context)
        {
            // Nếu người dùng đăng nhập, ta có thể lấy thông tin từ Claim hoặc session tương ứng (tùy thuộc vào cách triển khai)
            var username = httpContext.User?.FindFirst("CustomerID")?.Value;

            IQueryable<Shopcart> query = context.Shopcarts
                .Include(s => s.IdbookNavigation)
                    .ThenInclude(b => b.Images) // Bao gồm danh sách ảnh từ bảng Book
                .Include(s => s.IdbookNavigation)
                    .ThenInclude(b => b.IddiscountNavigation) // Bao gồm thông tin giảm giá
                .Include(s => s.IduserNavigation);

            if (!string.IsNullOrEmpty(username))
            {
                // Người dùng đã đăng nhập
                query = query.Where(s => s.IduserNavigation.UserName == username);
            }
            else
            {
                // Người dùng chưa đăng nhập, dùng session id
                var idsession = httpContext.Session.Id;
                query = query.Where(s => s.Idsession == idsession);
            }

            return query.ToList();
        }
        public static string ConvertToSlug(string text)
        {
            // Chuyển đổi Unicode sang không dấu
            text = text.Normalize(NormalizationForm.FormD);
            text = new string(text
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray());

            // Chuyển thành chữ thường
            text = text.ToLower();

            // Xóa ký tự không phải chữ cái và số
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

            // Xóa khoảng trắng
            text = text.Replace(" ", "-");

            return text;
        }
        public static bool XacMinhTaiKhoan(string receimail, string token)
        {
            if (_configuration == null)
            {
                throw new InvalidOperationException("Cấu hình chưa được khởi tạo.");
            }
            string verificationTemplate = _configuration["EmailSettings:VerificationLink"] ?? "";
            string verificationLink = string.Format(verificationTemplate, token);
            string to = string.IsNullOrEmpty(receimail) ? "jonepnix@gmail.com" : receimail;
            string subject = "Xác Minh Tài Khoản - JoneBookShop";
            string body = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px;'>
                <h2 style='color: #2d89ef;'>Xác Minh Tài Khoản</h2>
                <p>Xin chào,</p>
                <p>Bạn đã đăng ký tài khoản tại <strong>JoneBookShop</strong>. Vui lòng nhấp vào nút bên dưới để xác minh tài khoản của bạn:</p>
                <p style='text-align: center;'>
                    <a href='{verificationLink}' style='background-color: #2d89ef; color: white; text-decoration: none; padding: 10px 20px; border-radius: 5px; display: inline-block;'>
                        Xác Minh Tài Khoản
                    </a>
                </p>
                <p>Email có giá trị sử dụng trong <strong>10 phút</strong>, nếu bạn không yêu cầu đăng ký tài khoản, vui lòng bỏ qua email này.</p>
                <hr>
                <p style='font-size: 12px; color: gray;'>Cửa Hàng JoneBookShop<br>Email: jonepnix@gmail.com<br>Hotline: +84 865 358 784</p>
            </div>";
            string email = _configuration["EmailSettings:Email"];
            string pass = _configuration["EmailSettings:Password"];
            string host = _configuration["EmailSettings:Host"];
            int port = int.Parse(_configuration["EmailSettings:Port"]);
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(email, "Cửa Hàng JoneBookShop"); // Thay đổi tên hiển thị của người gửi
                    mail.To.Add(to); // Thêm địa chỉ người nhận
                    mail.Subject = subject; // Tiêu đề email
                    mail.Body = body; // Nội dung email
                    mail.IsBodyHtml = true; // Chỉ định rằng nội dung là HTML
                    using (SmtpClient smtp = new SmtpClient(host, port))
                    {
                        smtp.UseDefaultCredentials = false; // Không sử dụng thông tin đăng nhập mặc định
                        smtp.EnableSsl = true; // Bật SSL để kết nối an toàn
                        smtp.Credentials = new NetworkCredential(email, pass); // Sử dụng thông tin đăng nhập đúng
                        smtp.Send(mail); // Gửi email
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool XuatHoaDonDienTu(string receimail, int idOrder, DbjonebookshopContext _context)
        {
            try
            {
                // Lấy thông tin đơn hàng từ database
                var order = _context.Bookorders
                    .Include(o => o.IdvoucherNavigation)
                    .Include(o => o.IdtaxNavigation)
                    .Include(o => o.IdpaymentNavigation)
                    .Include(o => o.IduserNavigation)
                    .FirstOrDefault(o => o.IdbookOrder == idOrder);
                var orderDetails = _context.Orderdetails.Where(od => od.IdbookOrder == idOrder)
                    .Include(d => d.IdbookNavigation)
                    .ToList();

                if (order == null || orderDetails == null || orderDetails.Count == 0)
                {
                    // Không tìm thấy đơn hàng hoặc chi tiết đơn hàng
                    return false;
                }

                // Xử lý hiển thị Ngày giao hàng và Mã giảm giá
                string deliveryDateText = order.DeliveryDate != null ? Convert.ToDateTime(order.DeliveryDate).ToShortDateString() : "Đang Giao";
                string voucherText = !string.IsNullOrEmpty(order.IdvoucherNavigation.DiscountValue.ToString()) && order.IdvoucherNavigation.Code.StartsWith("DEFAULT") ? "Không áp dụng" : order.IdvoucherNavigation.DiscountValue + " " + order.IdvoucherNavigation.DiscountType;
                string name = order.IduserNavigation.Name;
                string tax = order.IdtaxNavigation.Tax1 + "%";
                // Tạo nội dung hoá đơn theo định dạng HTML
                string invoiceBody = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px;'>
            <h2 style='color: #2d89ef;'>Hoá Đơn Điện Tử</h2>
            <p><strong>Tên Khách Hàng:</strong> {name}</p>
            <p><strong>Ngày Đặt Hàng:</strong> {order.OrderDate.ToShortDateString()}</p>
            <p><strong>Ngày Giao Hàng:</strong> {deliveryDateText}</p>
            <p><strong>Thuế:</strong> {tax}</p>
            <p><strong>Mã Giảm Giá:</strong> {voucherText}</p>
            <p><strong>Phương Thức Thanh Toán:</strong> {order.IdpaymentNavigation.Type}</p>
            <p><strong>Địa Chỉ Giao Hàng:</strong> {order.Address}</p>
            <p><strong>Tổng Tiền:</strong> {order.TotalPrice.ToString("#,##0") + "VND"}</p>
            <hr />
            <h3>Chi Tiết Đơn Hàng</h3>
            <table style='width:100%; border-collapse: collapse;'>
                <thead>
                    <tr>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Sản phẩm</th>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Số lượng</th>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Đơn giá</th>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Thành tiền</th>
                    </tr>
                </thead>
                <tbody>";

                foreach (var item in orderDetails)
                {
                    var thanhTien = (Convert.ToDecimal(item.Quantity) * Convert.ToDecimal(item.Price)).ToString("#,##0");
                    var Tien = Convert.ToDecimal(item.Price).ToString("#,##0");
                    invoiceBody += $@"
                    <tr>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{item.IdbookNavigation.BookName}</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{item.Quantity}</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{Tien} VND</td>
                        <td style='border: 1px solid #ddd; padding: 8px;'>{thanhTien} VND</td>
                    </tr>";
                }

                invoiceBody += @"
                </tbody>
            </table>
            <p style='margin-top: 20px;'>Trân trọng,<br/>JoneBookShop</p>
        </div>";

                // Thông tin SMTP (giống như function xác minh tài khoản)
                string email = "jonebookshopverify@gmail.com";
                string pass = "chrv ydve gpsh woyy";
                string host = "smtp.gmail.com";
                int port = 587;
                string subject = "Hoá Đơn Điện Tử - JoneBookShop";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(email, "JoneBookShop");
                    mail.To.Add(receimail);
                    mail.Subject = subject;
                    mail.Body = invoiceBody;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(host, port))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential(email, pass);
                        smtp.Send(mail);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Có thể ghi log lỗi ex.Message nếu cần
                return false;
            }
        }

    }
}
