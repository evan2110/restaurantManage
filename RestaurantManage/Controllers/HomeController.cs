using Microsoft.AspNetCore.Mvc;
using RestaurantManage.Models;
using System.Diagnostics;

namespace RestaurantManage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string username = HttpContext.Session.GetString("UserName");
            string role = HttpContext.Session.GetString("Role");

            ViewBag.username = username;
            ViewBag.role = role;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
