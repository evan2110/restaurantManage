using Microsoft.AspNetCore.Mvc;
using RestaurantManage.DTOs;
using RestaurantManage.Models;

namespace RestaurantManage.Controllers;

public class AccountController : Controller
{
    private QuanLyNhaHangContext _context = new QuanLyNhaHangContext(); 
    // GET
    public IActionResult Index()
    {
        string username = HttpContext.Session.GetString("UserName");
        string role = HttpContext.Session.GetString("Role");

        ViewBag.username = username;
        ViewBag.role = role;
        if (username == null)
        {
            return View("~/Views/Shared/AuthenErr.cshtml");
        }

        Account account = _context.Accounts.SingleOrDefault(e => e.UserName == username);
        ViewBag.account = account;
        ViewBag.error = TempData["error"];
        return View();
    }
    
    [HttpPost]
    public IActionResult UpdateAccount(AccountDTO accountDto)
    {
        string username = accountDto.UserName.Trim();
        string password = accountDto.PassWord.Trim();
        string displayName = accountDto.DisplayName.Trim();
        string error = "";
        
        if (username == "" || string.IsNullOrEmpty(username))
        {
            error = "Tên đăng nhập không thể trống"; 
            TempData["error"] = error;
            return RedirectToAction("Index");
        }

        if (password == "" || string.IsNullOrEmpty(password))
        {
            error = "Mật khẩu không thể trống"; 
            TempData["error"] = error;
            return RedirectToAction("Index");
        }
        
        if (password.Length < 6)
        {
            error = "Độ dài password không đủ kí tự"; 
            TempData["error"] = error;
            return RedirectToAction("Index");
        }

        Account account = _context.Accounts.ToList().SingleOrDefault(e => e.UserName == username);
        account.DisplayName = displayName;
        account.PassWord = password;
        _context.Update(account);
        _context.SaveChanges();
        error = "Ok";

        TempData["error"] = error;
        return RedirectToAction("Index");
    }
}