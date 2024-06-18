using Microsoft.AspNetCore.Mvc;
using RestaurantManage.Models;

namespace RestaurantManage.Controllers;

public class TableController : Controller
{
    private QuanLyNhaHangContext _context = new QuanLyNhaHangContext(); 
    // GET
    public IActionResult Index(int? page)
    {
        string username = HttpContext.Session.GetString("UserName");
        string role = HttpContext.Session.GetString("Role");

        ViewBag.username = username;
        ViewBag.role = role;
        
        page = page ?? 1;
        List<TableFood> tableFoods = new List<TableFood>();
        List<TableFood> totalTable = new List<TableFood>();
        
        tableFoods = _context.TableFoods.Skip((page.Value - 1) * 8).Take(8).ToList();
        totalTable = _context.TableFoods.ToList();
        int countPage = (int)Math.Ceiling((double)totalTable.Count / 8);
        
        ViewBag.page = page;
        ViewBag.tableFoods = tableFoods;
        ViewBag.countPage = countPage;
        return View();
    }
}