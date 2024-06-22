namespace RestaurantManage.DTOs;

public class AccountManageDTO
{
    public string Check { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string PassWord { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public int accountType { get; set; } = 0;
}