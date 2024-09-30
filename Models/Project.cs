using System;
using System.Collections.Generic;

namespace Leon.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public string? ProjectName { get; set; }

    public string? BusinessOwner { get; set; }

    public string? InitiativeId { get; set; }

    public int? ResourceId { get; set; }

    public virtual Resource? Resource { get; set; }
}
