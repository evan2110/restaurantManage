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
        
        [HttpPost]
        public IActionResult Login(AccountDTO accountDto)
        {
            string username = accountDto.UserName.Trim();
            string password = accountDto.PassWord.Trim();
            string error = "";
            Account account = new Account();
            account = _context.Accounts.ToList().SingleOrDefault(e => e.UserName == username && e.PassWord == password);
            if (account == null)
            {
                error = "1";
            }
            else
            {
                HttpContext.Session.SetString("UserName", username);
                if (account.Type == 0)
                {
                    HttpContext.Session.SetString("Role", "Staff");
                }
                else
                {
                    HttpContext.Session.SetString("Role", "Admin");
                }
                error = "0";
            }
            
            TempData["error"] = error;
            if (error == "1")
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }
    }
}
