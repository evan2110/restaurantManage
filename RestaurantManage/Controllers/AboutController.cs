using Microsoft.AspNetCore.Mvc;

namespace RestaurantManage.Controllers;

public class AboutController : Controller
{
    // GET
    public IActionResult Index()
    {
        string username = HttpContext.Session.GetString("UserName");
        string role = HttpContext.Session.GetString("Role");

        ViewBag.username = username;
        ViewBag.role = role;
        return View();
    }
}