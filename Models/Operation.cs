using System;
using System.Collections.Generic;

namespace Leon.Models;

public partial class Operation
{
    public int OperationId { get; set; }

    public string? OperationName { get; set; }

    public string? BusinessOwner { get; set; }

    public int? ResourceId { get; set; }

    public virtual Resource? Resource { get; set; }
}
