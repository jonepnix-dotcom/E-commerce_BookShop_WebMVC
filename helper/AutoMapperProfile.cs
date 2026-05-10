using AutoMapper;
using TheLight_JoneBookShop_WebMVC.DTO;
using TheLight_JoneBookShop_WebMVC.Models;

namespace TheLight_JoneBookShop_WebMVC.helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterVM, Client>()
                .ForMember(dest => dest.Password, opt => opt.Ignore()).ReverseMap();
            CreateMap<Shopcart, ShopCartVM>()
            .ForMember(dest => dest.BookName, opt => opt.MapFrom(src => src.IdbookNavigation.BookName))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src =>
                src.IdbookNavigation.Images.FirstOrDefault()!.Name ?? "Logo_JP.png" // Tránh null
            ))
            .ForMember(dest => dest.DiscountValue, opt => opt.MapFrom(src =>
                (src.IdbookNavigation.IddiscountNavigation != null
                && src.IdbookNavigation.IddiscountNavigation.Status == true
                && src.IdbookNavigation.IddiscountNavigation.DiscountValue > 0
                && src.IdbookNavigation.IddiscountNavigation.StartDate <= DateTime.Today
                && src.IdbookNavigation.IddiscountNavigation.EndDate >= DateTime.Today)
                    ? src.IdbookNavigation.IddiscountNavigation.DiscountValue
                    : 0
            ))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
                (src.IdbookNavigation.IddiscountNavigation != null
                && src.IdbookNavigation.IddiscountNavigation.Status == true
                && src.IdbookNavigation.IddiscountNavigation.DiscountValue > 0
                && src.IdbookNavigation.IddiscountNavigation.StartDate <= DateTime.Today
                && src.IdbookNavigation.IddiscountNavigation.EndDate >= DateTime.Today)
                    ? src.IdbookNavigation.Price * (1 - src.IdbookNavigation.IddiscountNavigation.DiscountValue / 100)
                    : src.IdbookNavigation.Price
            ));
            CreateMap<Bookorder, OrderHistory>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdbookOrder))
           .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.IduserNavigation.Name))
           .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
           .ForMember(dest => dest.DeliveryDate, opt => opt.MapFrom(src => src.DeliveryDate ?? null))
           .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.IdtaxNavigation.Tax1 + "%"))
           .ForMember(dest => dest.Voucher, opt => opt.MapFrom(src => src.IdvoucherNavigation.Code + " | " + src.IdvoucherNavigation.DiscountValue+src.IdvoucherNavigation.DiscountType))
           .ForMember(dest => dest.Payment, opt => opt.MapFrom(src => src.IdpaymentNavigation.Type))
           .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
           .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.TotalPrice)); // Định dạng tiền tệ
            CreateMap<ShopCartVM, Orderdetail>()
            .ForMember(dest => dest.IdorderDetails, opt => opt.Ignore())
            .ForMember(dest => dest.IdbookOrder, opt => opt.Ignore())
            .ForMember(dest => dest.Idbook, opt => opt.MapFrom(src => src.Idbook))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.IdbookNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.IdbookOrderNavigation, opt => opt.Ignore());
        }
    }
}
