using TheLight_JoneBookShop_WebMVC.DTO;

namespace TheLight_JoneBookShop_WebMVC.Service
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExcute(IQueryCollection collection);
    }
}
