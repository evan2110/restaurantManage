using Microsoft.AspNetCore.Mvc;
using RestaurantManage.DTOs;
using RestaurantManage.Models;

namespace RestaurantManage.Controllers;

public class RegisterController : Controller
{
    private QuanLyNhaHangContext _context = new QuanLyNhaHangContext(); 
    // GET
    public IActionResult Index()
    {
        ViewBag.error = TempData["error"];
        return View();
    }
    
    [HttpPost]
    public IActionResult Register(AccountDTO accountDto)
    {
        string username = accountDto.UserName.Trim();
        string password = accountDto.PassWord.Trim();
        string error = "";

        if (_context.Accounts.ToList().SingleOrDefault(e => e.UserName == username) != null)
        {
            error = "1";
        }
        else
        {
            _context.Accounts.Add(new Account() { UserName = username, PassWord = password, Type = 0 });
            error = "0";
            _context.SaveChanges();
        }

        TempData["error"] = error;
        return RedirectToAction("Index");
    }
}