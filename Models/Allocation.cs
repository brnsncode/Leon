using System;
using System.Collections.Generic;

namespace Leon.Models;

public partial class Allocation
{
    public int AllocationId { get; set; }

    public int? TaskId { get; set; }

    public int? ResourceId { get; set; }

    public int? PercentageAllocated { get; set; }

    public virtual Resource? Resource { get; set; }

    public virtual Task? Task { get; set; }
}
