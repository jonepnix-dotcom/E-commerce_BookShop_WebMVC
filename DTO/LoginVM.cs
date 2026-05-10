using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheLight_JoneBookShop_WebMVC.DTO
{
    public class LoginVM
    {
        [DisplayName("Tên Đăng Nhập")]
        [Required(ErrorMessage = "Thiếu Tên Đăng Nhập")]
        [StringLength(30, ErrorMessage = "Tên Đăng Nhập từ 6 tới 30 ký tự.", MinimumLength = 6)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Không được nhập ký tự đặc biệt.")]
        public string UserName { get; set; } = null!;

        [DisplayName("Mật Khẩu")]
        [Required(ErrorMessage = "Thiếu Mật Khẩu")]
        [DataType(DataType.Password)]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Mật Khẩu có từ 6 tới 30 ký tự.")]
        public string Password { get; set; } = null!;
    }
}
