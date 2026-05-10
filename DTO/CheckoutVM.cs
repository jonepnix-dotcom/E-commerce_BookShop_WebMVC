using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TheLight_JoneBookShop_WebMVC.DTO
{
    public class CheckoutVM
    {
        [Required]
        public required int Amount {  get; set; }
        public int UserId {  get; set; }
        [DisplayName("Tên Khách Hàng")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Thiếu Địa Chỉ")]
        [StringLength(200, ErrorMessage = "Địa Chỉ phải có từ 6 tới 200 ký tự.", MinimumLength = 6)]
        [DisplayName("Địa Chỉ Giao Hàng")]
        public string? Address { get; set; }

        [DisplayName("Email")]
        public string? Email { get; set; }
    }
}
