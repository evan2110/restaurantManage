using Microsoft.AspNetCore.Mvc;

namespace RestaurantManage.Controllers;

public class AccountController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}