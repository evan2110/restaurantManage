using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManage.Models;

namespace RestaurantManage.Controllers;

public class HistoryController : Controller
{
    private QuanLyNhaHangContext _context = new QuanLyNhaHangContext(); 

    // GET
    public IActionResult Index()
    {
        string username = HttpContext.Session.GetString("UserName");
        string role = HttpContext.Session.GetString("Role");
        List<Bill> bills = new List<Bill>();
        bills = _context.Bills.Include(e => e.IdTableNavigation).OrderByDescending(e => e.DateCheckIn).ToList();
       
        ViewBag.username = username;
        ViewBag.role = role;
        ViewBag.bills = bills;
        
        return View();
    }
}