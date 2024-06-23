using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManage.DTOs;
using RestaurantManage.Models;
using OfficeOpenXml; // You may need to install the EPPlus package via NuGet
using System.IO;

namespace RestaurantManage.Controllers;

public class DashboardController : Controller
{
    private QuanLyNhaHangContext _context = new QuanLyNhaHangContext(); 

    // GET
    public IActionResult Index(string? mode, int? page, string? username, int? tableId, int? foodId, int? categoryId, int? billId)
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
            if (tableId != null)
            {
                var table = _context.TableFoods.SingleOrDefault(e => e.Id == tableId);
                ViewBag.table = table;
            }
            ViewBag.error = TempData["error"];
            ViewBag.mode = "Table";
        }else if (mode == "ManageFood")
        {
            page = page ?? 1;
            List<Food> foods = new List<Food>();
            List<Food> totalFood = new List<Food>();
        
            foods = _context.Foods.Include(e => e.IdCategoryNavigation).Skip((page.Value - 1) * 5).Take(5).ToList();
            totalFood = _context.Foods.ToList();
            int countPage = (int)Math.Ceiling((double)totalFood.Count / 5);

            ViewBag.page = page;
            ViewBag.foods = foods;
            ViewBag.countPage = countPage;
            ViewBag.mode = "ManageFood";
        }else if (mode == "Food")
        {
            List<FoodCategory> foodCategories = new List<FoodCategory>();
            foodCategories = _context.FoodCategories.ToList();

            if (foodId != null)
            {
                var food = _context.Foods.SingleOrDefault(e => e.Id == foodId);
                ViewBag.food = food;
            }
            ViewBag.foodCategories = foodCategories;
            ViewBag.error = TempData["error"];
            ViewBag.mode = "Food";
        }else if (mode == "ManageCategory")
        {
            page = page ?? 1;
            List<FoodCategory> categories = new List<FoodCategory>();
            List<FoodCategory> totalCategories = new List<FoodCategory>();
        
            categories = _context.FoodCategories.Skip((page.Value - 1) * 5).Take(5).ToList();
            totalCategories = _context.FoodCategories.ToList();
            int countPage = (int)Math.Ceiling((double)totalCategories.Count / 5);

            ViewBag.page = page;
            ViewBag.categories = categories;
            ViewBag.countPage = countPage;
            ViewBag.mode = "ManageCategory";
        }else if (mode == "Category")
        {
            if (categoryId != null)
            {
                var category = _context.FoodCategories.SingleOrDefault(e => e.Id == categoryId);
                ViewBag.category = category;
            }
            ViewBag.error = TempData["error"];
            ViewBag.mode = "Category";
        }else if (mode == "ManageBill")
        {
            page = page ?? 1;
            List<Bill> bills = new List<Bill>();
            List<Bill> totalBills = new List<Bill>();
        
            bills = _context.Bills.Include(e => e.IdTableNavigation).OrderByDescending(e => e.DateCheckIn).Skip((page.Value - 1) * 5).Take(5).ToList();
            totalBills = _context.Bills.ToList();
            int countPage = (int)Math.Ceiling((double)totalBills.Count / 5);

            ViewBag.page = page;
            ViewBag.bills = bills;
            ViewBag.countPage = countPage;
            ViewBag.mode = "ManageBill";
        }else if (mode == "Bill")
        {
            List<TableFood> tables = new List<TableFood>();
            tables = _context.TableFoods.Where(e => e.Status == "0").ToList();
            if (billId != null)
            {
                var bill = _context.Bills.SingleOrDefault(e => e.Id == billId);
                var table = _context.TableFoods.SingleOrDefault(e => e.Id == bill.IdTable);
                tables.Add(table);
                ViewBag.bill = bill;
            }

            ViewBag.tables = tables;
            ViewBag.error = TempData["error"];
            ViewBag.mode = "Bill";
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
    
    [HttpPost]
    public IActionResult FoodManage(FoodManageDTO foodManageDto)
    {
        string error = "";
        if (foodManageDto.FoodName.Trim() == "" || string.IsNullOrEmpty(foodManageDto.FoodName.Trim()))
        {
            error = "Tên món ăn không thể trống"; 
            TempData["error"] = error;
            return RedirectToAction("Index", "Dashboard", new { Mode = "Food" });
        }
        
        if (foodManageDto.FoodId != 0)
        {
            var food = _context.Foods.SingleOrDefault(e => e.Id == foodManageDto.FoodId);
            food.Name = foodManageDto.FoodName.Trim();
            food.IdCategory = foodManageDto.CategoryId;
            food.Price = foodManageDto.Price;

            _context.Update(food);
            _context.SaveChanges();
        }
        else
        {
            if (_context.Foods.ToList().SingleOrDefault(e => e.Name.ToLower().Trim() == foodManageDto.FoodName.ToLower().Trim()) != null)
            {
                error = "Tên món ăn đã tồn tại"; 
                TempData["error"] = error;
                return RedirectToAction("Index", "Dashboard", new { Mode = "Food" });
            }
            
            Food food = new Food()
            {
                Name = foodManageDto.FoodName.Trim(), IdCategory = foodManageDto.CategoryId,
                Price = foodManageDto.Price
            };
            
            _context.Add(food);
            _context.SaveChanges();
        }

        return RedirectToAction("Index", "Dashboard", new { Mode = "ManageFood" });
    }
    
    [HttpPost]
    public IActionResult CategoryManage(CategoryManageDTO categoryManageDto)
    {
        string error = "";
        if (categoryManageDto.CategoryName.Trim() == "" || string.IsNullOrEmpty(categoryManageDto.CategoryName.Trim()))
        {
            error = "Tên danh mục không thể trống"; 
            TempData["error"] = error;
            return RedirectToAction("Index", "Dashboard", new { Mode = "Category" });
        }
        
        if (categoryManageDto.CategoryId != 0)
        {
            var category = _context.FoodCategories.SingleOrDefault(e => e.Id == categoryManageDto.CategoryId);
            category.Name = categoryManageDto.CategoryName.Trim();

            _context.Update(category);
            _context.SaveChanges();
        }
        else
        {
            if (_context.FoodCategories.ToList().SingleOrDefault(e => e.Name.ToLower().Trim() == categoryManageDto.CategoryName.ToLower().Trim()) != null)
            {
                error = "Tên danh mục đã tồn tại"; 
                TempData["error"] = error;
                return RedirectToAction("Index", "Dashboard", new { Mode = "Category" });
            }
            
            FoodCategory category = new FoodCategory()
            {
                Name = categoryManageDto.CategoryName.Trim()
            };
            
            _context.Add(category);
            _context.SaveChanges();
        }

        return RedirectToAction("Index", "Dashboard", new { Mode = "ManageCategory" });
    }
    
    [HttpPost]
    public IActionResult BillManage(BillManageDTO billManageDto)
    {
        string error = "";
        
        if (billManageDto.BillId != 0)
        {
            var bill = _context.Bills.SingleOrDefault(e => e.Id == billManageDto.BillId);
            if (bill.IdTable != billManageDto.TableId)
            {
                var table = _context.TableFoods.SingleOrDefault(e => e.Id == bill.IdTable);
                table.Status = "0";
                _context.Update(table);
                _context.SaveChanges();
                
                var tableNew = _context.TableFoods.SingleOrDefault(e => e.Id == billManageDto.TableId);
                tableNew.Status = "1";
                _context.Update(tableNew);
                _context.SaveChanges();
            }
            bill.IdTable = billManageDto.TableId;
            if (billManageDto.Status == 1)
            {
                bill.DateCheckOut = DateTime.Now;
            }
            
            bill.Status = billManageDto.Status;
            bill.TotalPrice = billManageDto.Price;
            
            _context.Update(bill);
            _context.SaveChanges();
        }
        else
        {
            var table = _context.TableFoods.SingleOrDefault(e => e.Id == billManageDto.TableId);
            table.Status = "1";
            _context.Update(table);
            _context.SaveChanges();
            
            Bill bill = new Bill()
            {
                DateCheckIn = DateTime.Now,
                IdTable = billManageDto.TableId,
                Status = billManageDto.Status,
                TotalPrice = billManageDto.Price
            };

            if (billManageDto.Status == 1)
            {
                bill.DateCheckOut = DateTime.Now;
            }
            
            _context.Add(bill);
            _context.SaveChanges();
        }

        return RedirectToAction("Index", "Dashboard", new { Mode = "ManageBill" });
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
            var table = _context.TableFoods.Include(e => e.Bills).ThenInclude(e => e.BillInfos).SingleOrDefault(e => e.Id == int.Parse(id));
            foreach (var item in table.Bills)
            {
                _context.BillInfos.RemoveRange(item.BillInfos);
                _context.SaveChanges();
            }

            _context.Bills.RemoveRange(table.Bills);
            _context.SaveChanges();
            
            _context.TableFoods.Remove(table);
            _context.SaveChanges();
            
            return RedirectToAction("Index", "Dashboard", new { Mode = "ManageTable" });
        }else if (mode == "food")
        {
            var food = _context.Foods.Include(e => e.BillInfos).SingleOrDefault(e => e.Id == int.Parse(id));
       
            _context.BillInfos.RemoveRange(food.BillInfos);
            _context.SaveChanges();

            _context.Foods.Remove(food);
            _context.SaveChanges();
            return RedirectToAction("Index", "Dashboard", new { Mode = "ManageFood" });
        }else if (mode == "category")
        {
            var category = _context.FoodCategories.Include(e => e.Foods).ThenInclude(e => e.BillInfos).SingleOrDefault(e => e.Id == int.Parse(id));

            foreach (var item in category.Foods)
            {
                _context.BillInfos.RemoveRange(item.BillInfos);
                _context.SaveChanges();
            }
            
            _context.Foods.RemoveRange(category.Foods);
            _context.SaveChanges();
            
            _context.FoodCategories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction("Index", "Dashboard", new { Mode = "ManageCategory" });
        }else if (mode == "bill")
        {
            var bill = _context.Bills.Include(e => e.BillInfos).SingleOrDefault(e => e.Id == int.Parse(id));
            if (bill.Status == 0)
            {
                var table = _context.TableFoods.SingleOrDefault(e => e.Id == bill.IdTable);
                table.Status = "0";
                _context.Update(table);
                _context.SaveChanges();
            }
            _context.BillInfos.RemoveRange(bill.BillInfos);
            _context.SaveChanges();
            
            _context.Bills.Remove(bill);
            _context.SaveChanges();
            return RedirectToAction("Index", "Dashboard", new { Mode = "ManageBill" });
        }
        return null;
    }
    
    [HttpGet]
    public IActionResult ExportToExcel()
    {
        var bills = _context.Bills.Include(e => e.IdTableNavigation).ToList();
        
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Hóa đơn");
            worksheet.Cells["A1"].Value = "Mã hóa đơn";
            worksheet.Cells["B1"].Value = "Ngày tạo";
            worksheet.Cells["C1"].Value = "Ngày thanh toán";
            worksheet.Cells["D1"].Value = "Tên bàn";
            worksheet.Cells["E1"].Value = "Trạng thái";
            worksheet.Cells["F1"].Value = "Tổng tiền";

            for (int i = 0; i < bills.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = bills[i].Id;
                worksheet.Cells[i + 2, 2].Value = bills[i].DateCheckIn.ToString("yyyy-MM-dd");
                worksheet.Cells[i + 2, 3].Value = bills[i].DateCheckOut?.ToString("yyyy-MM-dd");
                worksheet.Cells[i + 2, 4].Value = bills[i].IdTableNavigation.Name;
                worksheet.Cells[i + 2, 5].Value = (bills[i].Status == 0) ? "Trống" : "Hết chổ";
                worksheet.Cells[i + 2, 6].Value = bills[i].TotalPrice;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Hóa_đơn.xlsx");
        }
    }
}