using RestaurantManage.Models;

namespace RestaurantManage.DTOs;

public class BillDTO
{
    public List<FoodOrderDTO> FoodOrderDtos { get; set; } = null!;
    public double Total { get; set; } = 0!;
    public int tableId { get; set; } = 0!;
}