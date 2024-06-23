using Microsoft.AspNetCore.Mvc;
using RestaurantManage.Models;
using System.Diagnostics;

namespace RestaurantManage.Controllers
{
    public class HomeController : Controller
    {
        private QuanLyNhaHangContext _context = new QuanLyNhaHangContext(); 
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<Food> foods = _context.Foods.OrderByDescending(e => e.Id).Take(8).ToList();
            ViewBag.foods = foods;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
