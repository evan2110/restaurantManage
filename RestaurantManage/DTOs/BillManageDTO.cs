namespace RestaurantManage.DTOs;

public class BillManageDTO
{
    public int BillId { get; set; } = 0;
    public int TableId { get; set; } = 0;
    public int Status { get; set; }
    public double Price { get; set; } = 0;
}