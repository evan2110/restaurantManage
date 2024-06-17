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
        string error = "";
        string username = accountDto.UserName.Trim();
        string password = accountDto.PassWord.Trim();
        string rePassword = accountDto.RePassword.Trim();
        
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
        
        if (rePassword == "" || string.IsNullOrEmpty(rePassword))
        {
            error = "Mật khẩu nhập lại không thể trống"; 
            TempData["error"] = error;
            return RedirectToAction("Index");
        }
        
        if (password != rePassword)
        {
            error = "Mật khẩu nhập lại không trùng khớp"; 
            TempData["error"] = error;
            return RedirectToAction("Index");
        }

        if (password.Length < 6)
        {
            error = "Độ dài password không đủ kí tự"; 
            TempData["error"] = error;
            return RedirectToAction("Index");
        }

        if (rePassword.Length < 6)
        {
            error = "Độ dài rePassword không đủ kí tự"; 
            TempData["error"] = error;
            return RedirectToAction("Index");
        }

        if (_context.Accounts.ToList().SingleOrDefault(e => e.UserName == username) != null)
        {
            error = "Tài khoản đã tồn tại"; 
            TempData["error"] = error;
            return RedirectToAction("Index");
        }
        
        _context.Accounts.Add(new Account() { UserName = username, PassWord = password, Type = 0 });
        error = "Ok"; //Trường hợp OK
        _context.SaveChanges();

        TempData["error"] = error;
        return RedirectToAction("Index");
    }
}