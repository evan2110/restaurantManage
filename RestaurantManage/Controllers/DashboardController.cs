using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManage.DTOs;
using RestaurantManage.Models;

namespace RestaurantManage.Controllers;

public class DashboardController : Controller
{
    private QuanLyNhaHangContext _context = new QuanLyNhaHangContext(); 

    // GET
    public IActionResult Index(string? mode, int? page, string? username, int? tableId)
    {
        if (mode == "Dashboard" || mode == null)
        {
            double? totalReveneu;
            int? totalFoods;
            int? totalCust;
            totalReveneu = _context.Bills.ToList().Sum(e => e.TotalPrice);
            totalFoods = _context.Bills
                .Where(e => e.Status == 1)
                .SelectMany(bill => bill.BillInfos)
                .Sum(billInfo => billInfo.Count);
            totalCust = _context.Bills.Count();
            List<RateReveneuDTO> reveneuDtos = new List<RateReveneuDTO>();
            int year = DateTime.Now.Year;
            for (int i = 1; i <= 12; i++)
            {
                reveneuDtos.Add(new RateReveneuDTO(){Month = i, Reveneu = _context.Bills.
                    Where(e => e.DateCheckOut.Value.Month == i && e.DateCheckOut.Value.Year == year).Sum(e => e.TotalPrice)});
            }

            List<TopFoodDTO> topFoodDtos = new List<TopFoodDTO>();
            var topFoods = (from bill in _context.BillInfos
                join food in _context.Foods on bill.IdFood equals food.Id
                group new { food, bill } by new { food.Name, food.Price } into grp
                orderby grp.Sum(x => x.bill.Count) descending
                select new TopFoodDTO
                {
                    FoodName = grp.Key.Name,
                    Quantity = grp.Sum(x => x.bill.Count),
                    Reveneu = grp.Sum(x => x.bill.Count * x.food.Price)
                }).Take(3).ToList();

            ViewBag.topFoods = topFoods;
            ViewBag.totalFoods = totalFoods;
            ViewBag.totalReveneu = totalReveneu;
            ViewBag.totalCust = totalCust;
            ViewBag.reveneuDtos = reveneuDtos;
            ViewBag.mode = "Dashboard";
        }else if (mode == "ManageAcc")
        {
            page = page ?? 1;
            List<Account> accounts = new List<Account>();
            List<Account> totalAccount = new List<Account>();
        
            accounts = _context.Accounts.Skip((page.Value - 1) * 5).Take(5).ToList();
            totalAccount = _context.Accounts.ToList();
            int countPage = (int)Math.Ceiling((double)totalAccount.Count / 5);
            
            ViewBag.page = page;
            ViewBag.accounts = accounts;
            ViewBag.countPage = countPage;
            ViewBag.mode = "ManageAcc";
        }else if (mode == "Account")
        {
            if (username != null)
            {
                var account = _context.Accounts.SingleOrDefault(e => e.UserName == username);
                ViewBag.account = account;
            }
            ViewBag.error = TempData["error"];
            ViewBag.mode = "Account";
        }else if (mode == "ManageTable")
        {
            page = page ?? 1;
            List<TableFood> tables = new List<TableFood>();
            List<TableFood> totalTable = new List<TableFood>();
        
            tables = _context.TableFoods.Skip((page.Value - 1) * 5).Take(5).ToList();
            totalTable = _context.TableFoods.ToList();
            int countPage = (int)Math.Ceiling((double)totalTable.Count / 5);
            
            ViewBag.page = page;
            ViewBag.tables = tables;
            ViewBag.countPage = countPage;
            ViewBag.mode = "ManageTable";
        }else if (mode == "Table")
        {
            if (tableId != 0)
            {
                var table = _context.TableFoods.SingleOrDefault(e => e.Id == tableId);
                ViewBag.table = table;
            }
            ViewBag.error = TempData["error"];
            ViewBag.mode = "Table";
        }
        return View();
    }
    
    [HttpPost]
    public IActionResult AccountManage(AccountManageDTO accountManageDto)
    {
        string error = "";
        if (accountManageDto.UserName.Trim() == "" || string.IsNullOrEmpty(accountManageDto.UserName.Trim()))
        {
            error = "Tên đăng nhập không thể trống"; 
            TempData["error"] = error;
            return RedirectToAction("Index", "Dashboard", new { Mode = "Account" });
        }
        
        if (accountManageDto.PassWord.Trim() == "" || string.IsNullOrEmpty(accountManageDto.PassWord.Trim()))
        {
            error = "Mật khẩu không thể trống"; 
            TempData["error"] = error;
            return RedirectToAction("Index", "Dashboard", new { Mode = "Account" });
        }
        
        if (accountManageDto.PassWord.Trim().Length < 6)
        {
            error = "Độ dài password không đủ kí tự"; 
            TempData["error"] = error;
            return RedirectToAction("Index", "Dashboard", new { Mode = "Account" });
        }
        
        if (accountManageDto.Check != null)
        {
            var account = _context.Accounts.SingleOrDefault(e => e.UserName == accountManageDto.UserName);
            account.DisplayName = accountManageDto.DisplayName.Trim();
            account.PassWord = accountManageDto.PassWord.Trim();
            account.Type = accountManageDto.accountType;

            _context.Update(account);
            _context.SaveChanges();
        }
        else
        {
            if (_context.Accounts.ToList().SingleOrDefault(e => e.UserName.ToLower().Trim() == accountManageDto.UserName.ToLower().Trim()) != null)
            {
                error = "Tài khoản đã tồn tại"; 
                TempData["error"] = error;
                return RedirectToAction("Index", "Dashboard", new { Mode = "Account" });
            }
            
            Account account = new Account()
            {
                UserName = accountManageDto.UserName.Trim(), DisplayName = accountManageDto.DisplayName.Trim(),
                PassWord = accountManageDto.PassWord.Trim(), Type = accountManageDto.accountType
            };
            
            _context.Add(account);
            _context.SaveChanges();
        }

        return RedirectToAction("Index", "Dashboard", new { Mode = "ManageAcc" });
    }
    
    [HttpPost]
    public IActionResult TableManage(TableManageDTO tableManageDto)
    {
        string error = "";
        if (tableManageDto.Name.Trim() == "" || string.IsNullOrEmpty(tableManageDto.Name.Trim()))
        {
            error = "Tên bàn không thể trống"; 
            TempData["error"] = error;
            return RedirectToAction("Index", "Dashboard", new { Mode = "Table" });
        }
        
        if (tableManageDto.TableId != 0)
        {
            var table = _context.TableFoods.SingleOrDefault(e => e.Id == tableManageDto.TableId);
            table.Name = tableManageDto.Name.Trim();
            table.Status = tableManageDto.Status.Trim();

            _context.Update(table);
            _context.SaveChanges();
        }
        else
        {
            if (_context.TableFoods.ToList().SingleOrDefault(e => e.Name.ToLower().Trim() == tableManageDto.Name.ToLower().Trim()) != null)
            {
                error = "Tên bàn đã tồn tại"; 
                TempData["error"] = error;
                return RedirectToAction("Index", "Dashboard", new { Mode = "Table" });
            }
            
            TableFood table = new TableFood()
            {
                Name = tableManageDto.Name.Trim(), Status = tableManageDto.Status.Trim(),
            };
            
            _context.Add(table);
            _context.SaveChanges();
        }

        return RedirectToAction("Index", "Dashboard", new { Mode = "ManageTable" });
    }
    
    [HttpGet]
    public IActionResult Remove(string? id, string? mode)
    {
        if (mode == "account")
        {
            var account = _context.Accounts.SingleOrDefault(e => e.UserName == id);
       
            _context.Accounts.Remove(account);
            _context.SaveChanges();
            return RedirectToAction("Index", "Dashboard", new { Mode = "ManageAcc" });
        }else if (mode == "table")
        {
            var table = _context.TableFoods.SingleOrDefault(e => e.Id == int.Parse(id));
       
            _context.TableFoods.Remove(table);
            _context.SaveChanges();
            return RedirectToAction("Index", "Dashboard", new { Mode = "ManageTable" });
        }
        return null;
    }
}