using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class FloorPlan
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Color { get; set; } = null!;

    public virtual ICollection<FloorPlanTable> FloorPlanTables { get; set; } = new List<FloorPlanTable>();
}
