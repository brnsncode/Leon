using System;
using System.Collections.Generic;

namespace Leon.Models;

public partial class ResourceCapacityView
{
    public int ResourceId { get; set; }

    public string? EmployeeName { get; set; }

    public decimal? TotalCapacity { get; set; }
}
