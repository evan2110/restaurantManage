using Microsoft.AspNetCore.Mvc;

namespace RestaurantManage.Controllers;

public class AboutController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}