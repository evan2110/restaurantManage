using Microsoft.AspNetCore.Mvc;
using RestaurantManage.DTOs;
using RestaurantManage.Models;

namespace RestaurantManage.Controllers;

public class CartController : Controller
{
    private QuanLyNhaHangContext _context = new QuanLyNhaHangContext(); 
    // GET
    public IActionResult Index(string? tableNm, int? tableId, int? idCategory)
    {
        tableNm = tableNm ?? "";
        tableId = tableId ?? 0;
        idCategory = idCategory ?? 0;
        
        List<FoodCategory> foodCategories = new List<FoodCategory>();
        List<Food> foods = new List<Food>();
        foodCategories = _context.FoodCategories.ToList();
        if (idCategory == 0)
        {
            foods = _context.Foods.ToList();
        }
        else
        {
            foods = _context.Foods.Where(e => e.IdCategory == idCategory).ToList();
        }

        ViewBag.foods = foods;
        ViewBag.foodCategories = foodCategories;
        ViewBag.tableNm = tableNm;
        ViewBag.tableId = tableId;
        return View();
    }
    
    [HttpPost]
    public IActionResult AddBill(BillDTO billDto)
    {
        _context.Bills.Add(new Bill()
            { DateCheckIn = DateTime.Now, IdTable = billDto.tableId, Status = 0, TotalPrice = billDto.Total });
        _context.SaveChanges();
        var tableFood = _context.TableFoods.SingleOrDefault(e => e.Id == billDto.tableId);
        tableFood.Status = "1";
        _context.Update(tableFood);
        _context.SaveChanges();
        /*foreach (var item in billDto.FoodOrderDtos)
        {
            if (item.Quantity > 0)
            {
                _context.BillInfos.Add(
            }
        }*/
        
        return RedirectToAction("Index", "Home");
    }
}