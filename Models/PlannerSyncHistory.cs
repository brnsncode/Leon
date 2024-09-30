using System;
using System.Collections.Generic;

namespace Leon.Models;

public partial class PlannerSyncHistory
{
    public int SyncId { get; set; }

    public DateTime? SyncDate { get; set; }

    public string? SyncReason { get; set; }
}
