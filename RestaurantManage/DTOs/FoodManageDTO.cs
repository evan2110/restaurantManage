namespace RestaurantManage.DTOs;

public class FoodManageDTO
{
    public int FoodId { get; set; } = 0;
    public string FoodName { get; set; }
    public int CategoryId { get; set; } = 0;
    public double Price { get; set; } = 0;
}