using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class FloorPlanTable
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int FloorPlanId { get; set; }

    public double PositionX { get; set; }

    public double PositionY { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }

    public int IsRound { get; set; }

    public virtual FloorPlan FloorPlan { get; set; } = null!;
}
