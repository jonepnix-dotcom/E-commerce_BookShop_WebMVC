using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using TheLight_JoneBookShop_WebMVC.helper;

namespace TheLight_JoneBookShop_WebMVC.DTO
{
    public class RegisterVM
    {
        [DisplayName("Tên Đăng Nhập")]
        [Required(ErrorMessage = "Thiếu Tên Đăng Nhập")]
        [Remote(action: "CheckUserName", controller: "KhachHang", ErrorMessage = "Tên tài khoản đã tồn tại.")]
        [StringLength(30, ErrorMessage = "Tên Đăng Nhập từ 6 tới 30 ký tự.", MinimumLength = 6)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Không được nhập ký tự đặc biệt.")]
        public string? UserName { get; set; }

        [DisplayName("Mật Khẩu")]
        [Required(ErrorMessage = "Thiếu Mật Khẩu")]
        [DataType(DataType.Password)]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Mật Khẩu phải có từ 6 tới 30 ký tự.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[a-zA-Z\d!@#$%^&*]+$", ErrorMessage = "Mật Khẩu Chưa Đủ Mạnh")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Thiếu Tên Người Dùng")]
        [DisplayName("Tên Người Dùng")]
        [StringLength(50, ErrorMessage = "Tên Người Dùng phải có từ 6 tới 30 ký tự.", MinimumLength = 6)]
        [RegularExpression(@"^[a-zA-Z0-9\u00C0-\u024F\u1E00-\u1EFF\s]+$", ErrorMessage = "Không được nhập ký tự đặc biệt.")]

        public string? Name { get; set; }

        [Required(ErrorMessage = "Thiếu Địa Chỉ")]
        [DisplayName("Địa Chỉ")]
        [StringLength(200, ErrorMessage = "Địa chỉ phải có từ 6 tới 200 ký tự.", MinimumLength = 6)]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Thiếu Email")]
        [Remote(action: "CheckEmail", controller: "KhachHang", ErrorMessage = "Email đã tồn tại.")]
        [DisplayName("Email")]
        [EmailAddress(ErrorMessage = "Sai Định Dạng Email")]
        [StringLength(100, ErrorMessage = "Email không được quá 100 ký tự.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Thiếu Số Điện Thoại")]
        [DisplayName("Số Điện Thoại")]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Số Điện Thoại phải có từ 10 đến 15 chữ số.")]
        public string? Phones { get; set; }

        [Required(ErrorMessage = "Thiếu Ngày Sinh")]
        [DisplayName("Ngày Sinh")]
        [DataType(DataType.Date)]
        public DateOnly? Birthday { get; set; }
    }

}
