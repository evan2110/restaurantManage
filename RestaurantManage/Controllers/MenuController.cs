using Microsoft.AspNetCore.Mvc;
using RestaurantManage.Models;

namespace RestaurantManage.Controllers;

public class MenuController : Controller
{
    private QuanLyNhaHangContext _context = new QuanLyNhaHangContext(); 
    // GET
    public IActionResult Index(int? idCategory, int? page)
    {
        idCategory = idCategory ?? 0;
        page = page ?? 1;
        int totalTable;
        int countPage;
        
        List<FoodCategory> foodCategories = new List<FoodCategory>();
        List<Food> foods = new List<Food>();
        foodCategories = _context.FoodCategories.ToList();
        if (idCategory == 0)
        {
            foods = _context.Foods.Skip((page.Value - 1) * 8).Take(8).ToList();
            totalTable = _context.Foods.ToList().Count;
        }
        else
        {
            foods = _context.Foods.Where(e => e.IdCategory == idCategory).Skip((page.Value - 1) * 8).Take(8).ToList();
            totalTable = _context.Foods.Where(e => e.IdCategory == idCategory).ToList().Count;
        }
        countPage = (int)Math.Ceiling((double)totalTable / 8);

        ViewBag.idCategory = idCategory;
        ViewBag.page = page;
        ViewBag.countPage = countPage;
        ViewBag.foods = foods;
        ViewBag.foodCategories = foodCategories;
        return View();
    }
}