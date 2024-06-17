using Microsoft.AspNetCore.Mvc;
using RestaurantManage.DTOs;
using RestaurantManage.Models;

namespace RestaurantManage.Controllers
{
    public class LoginController : Controller
    {
        private QuanLyNhaHangContext _context = new QuanLyNhaHangContext(); 
        public IActionResult Index()
        {
            ViewBag.error = TempData["error"];
            return View();
        }
        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        
        [HttpPost]
        public IActionResult Login(AccountDTO accountDto)
        {
            string username = accountDto.UserName.Trim();
            string password = accountDto.PassWord.Trim();
            string error = "";
            Account account = new Account();
            account = _context.Accounts.ToList().SingleOrDefault(e => e.UserName == username && e.PassWord == password);
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
            
            if (account == null)
            {
                error = "Tên đăng nhập hoặc mật khẩu không đúng!";
                TempData["error"] = error;
                return RedirectToAction("Index");
            }
            HttpContext.Session.SetString("UserName", username);
            if (account.Type == 0)
            {
                HttpContext.Session.SetString("Role", "Staff");
            }
            else
            {
                HttpContext.Session.SetString("Role", "Admin");
            }
            
            return RedirectToAction("Index", "Home");
        }
    }
}
