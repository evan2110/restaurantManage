using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantManage.DTOs;
using RestaurantManage.Models;

namespace RestaurantManage.Controllers;

public class DashboardController : Controller
{
    private QuanLyNhaHangContext _context = new QuanLyNhaHangContext(); 

    // GET
    public IActionResult Index(string? Mode)
    {
        if (Mode == "Dashboard" || Mode == null)
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
        }else if (Mode == "ManageAcc")
        {
            ViewBag.mode = "ManageAcc";
        }
        return View();
    }
}