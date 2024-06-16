using System;
using System.Collections.Generic;

namespace RestaurantManage.Models;

public partial class Account
{
    public string UserName { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string PassWord { get; set; } = null!;

    public int Type { get; set; }
}
