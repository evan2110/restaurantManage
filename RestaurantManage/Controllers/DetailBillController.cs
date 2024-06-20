using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManage.Models;

namespace RestaurantManage.Controllers;

public class DetailBillController : Controller
{
    private QuanLyNhaHangContext _context = new QuanLyNhaHangContext(); 
    // GET
    public IActionResult Index(int? id)
    {
        id = id ?? 0;
        var bill = _context.Bills.Include(e => e.BillInfos).ThenInclude(e => e.IdFoodNavigation).SingleOrDefault(e => e.Id == id);
        var tables = _context.TableFoods.Where(e => e.Status == "0").ToList();
        
        ViewBag.bill = bill;
        ViewBag.tables = tables;
        return View();
    }
    
    [HttpPost]
    public IActionResult ChangeTable(int idBill, int idTableChange)
    {
        if (idTableChange != 0)
        {
            var bill = _context.Bills.SingleOrDefault(e => e.Id == idBill);
            bill.IdTable = idTableChange;
            _context.Update(bill);
            _context.SaveChanges();
        }
        return RedirectToAction("Index", "History");
    }
}