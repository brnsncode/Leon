using System;
using System.Collections.Generic;

namespace Leon.Models;

public partial class ResourceProjectTask
{
    public string? ResourceName { get; set; }

    public string? ProjectName { get; set; }

    public string? TaskName { get; set; }

    public int? Capacity { get; set; }
}
