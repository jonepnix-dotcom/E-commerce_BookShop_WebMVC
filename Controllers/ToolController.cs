using Microsoft.AspNetCore.Mvc;

namespace TheLight_JoneBookShop_WebMVC.Controllers
{
    public class ToolController : Controller
    {
        [HttpGet]
        [Route("Loi-thong-tin")]
        public IActionResult ErrorView()
        {
            return View();
        }
        [HttpGet]
        [Route("thong-tin-thanh-toan")]
        public IActionResult PaymentSuccess()
        {
            return View();
        }
    }
}
