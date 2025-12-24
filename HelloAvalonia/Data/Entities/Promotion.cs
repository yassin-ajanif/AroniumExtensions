using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class Promotion
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? EndTime { get; set; }

    public int DaysOfWeek { get; set; }

    public int IsEnabled { get; set; }

    public virtual ICollection<PromotionItem> PromotionItems { get; set; } = new List<PromotionItem>();
}
