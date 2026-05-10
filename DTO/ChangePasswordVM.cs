using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TheLight_JoneBookShop_WebMVC.DTO
{
    public class ChangePasswordVM
    {
        [DisplayName("Mật Khẩu")]
        [Required(ErrorMessage = "Thiếu Mật Khẩu")]
        [DataType(DataType.Password)]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Mật Khẩu phải có từ 6 tới 30 ký tự.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[a-zA-Z\d!@#$%^&*]+$", ErrorMessage = "Mật Khẩu Chưa Đủ Mạnh")]
        public string? Password { get; set; }

        [DisplayName("Nhập Lại Mật Khẩu")]
        [Required(ErrorMessage = "Thiếu Xác Nhận Mật Khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không trùng khớp.")]
        public string? RePassword { get; set; }
    }
}
