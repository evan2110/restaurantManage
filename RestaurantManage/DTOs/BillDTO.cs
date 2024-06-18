using RestaurantManage.Models;

namespace RestaurantManage.DTOs;

public class BillDTO
{
    public string Foods { get; set; } = null!;
    public double Total { get; set; } = 0!;
    public int tableId { get; set; } = 0!;
}