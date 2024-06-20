using Microsoft.AspNetCore.Mvc;

namespace RestaurantManage.Controllers;

public class ContactController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}