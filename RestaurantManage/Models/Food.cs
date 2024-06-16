using System;
using System.Collections.Generic;

namespace RestaurantManage.Models;

public partial class Food
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int IdCategory { get; set; }

    public double Price { get; set; }

    public virtual ICollection<BillInfo> BillInfos { get; set; } = new List<BillInfo>();

    public virtual FoodCategory IdCategoryNavigation { get; set; } = null!;
}
