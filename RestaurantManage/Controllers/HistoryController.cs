using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManage.Models;

namespace RestaurantManage.Controllers;

public class HistoryController : Controller
{
    private QuanLyNhaHangContext _context = new QuanLyNhaHangContext(); 

    // GET
    public IActionResult Index(int? page)
    {
        page = page ?? 1;
        List<Bill> bills = new List<Bill>();
        List<Bill> totalBills = new List<Bill>();
        
        totalBills = _context.Bills.ToList();
        int countPage = (int)Math.Ceiling((double)totalBills.Count / 8);
        
        
        bills = _context.Bills.Include(e => e.IdTableNavigation).OrderByDescending(e => e.Id).Skip((page.Value - 1) * 10).Take(10).ToList();
       
        ViewBag.countPage = countPage;
        ViewBag.page = page;
        ViewBag.bills = bills;
        
        return View();
    }
    
    [HttpPost]
    public IActionResult CancelBill(int id)
    {
        var bill = _context.Bills.SingleOrDefault(e => e.Id == id);
        bill.Status = 2;
        _context.Update(bill);
        _context.SaveChanges();
        
        var table = _context.TableFoods.SingleOrDefault(e => e.Id == bill.IdTable);
        table.Status = "0";
        _context.Update(table);
        _context.SaveChanges();
        
        return RedirectToAction("Index", "History");
    }
    
    [HttpPost]
    public IActionResult ConfirmBill(int id)
    {
        var bill = _context.Bills.SingleOrDefault(e => e.Id == id);
        bill.Status = 1;
        bill.DateCheckOut = DateTime.Now;
        _context.Update(bill);
        _context.SaveChanges();
        
        var table = _context.TableFoods.SingleOrDefault(e => e.Id == bill.IdTable);
        table.Status = "0";
        _context.Update(table);
        _context.SaveChanges();
        
        return RedirectToAction("Index", "History");
    }
}