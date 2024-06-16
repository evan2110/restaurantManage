using Microsoft.AspNetCore.Mvc;

namespace RestaurantManage.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
