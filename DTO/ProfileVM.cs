using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TheLight_JoneBookShop_WebMVC.DTO
{
    public class ProfileVM
    {

        [Required(ErrorMessage = "Thiếu Tên Người Dùng")]
        [DisplayName("Tên Người Dùng")]
        [StringLength(50, ErrorMessage = "Tên Người Dùng phải có từ 6 tới 30 ký tự.", MinimumLength = 6)]
        [RegularExpression(@"^[a-zA-Z0-9\u00C0-\u024F\u1E00-\u1EFF\s]+$", ErrorMessage = "Không được nhập ký tự đặc biệt.")]

        public string? Name { get; set; }

        [Required(ErrorMessage = "Thiếu Địa Chỉ")]
        [DisplayName("Địa Chỉ")]
        [StringLength(200, ErrorMessage = "Địa Chỉ phải có từ 6 tới 200 ký tự.", MinimumLength = 6)]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Thiếu Email")]
        [Remote(action: "CheckEmailOrder", controller: "KhachHang", ErrorMessage = "Email đã tồn tại.")]
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
