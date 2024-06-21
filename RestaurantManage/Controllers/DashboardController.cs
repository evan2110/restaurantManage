using Microsoft.AspNetCore.Mvc;

namespace RestaurantManage.Controllers;

public class DashboardController : Controller
{
    // GET
    public IActionResult Index(string? Mode)
    {
        if (Mode == "Dashboard" || Mode == null)
        {
            ViewBag.Mode = "Dashboard";
        }
        return View();
    }
}