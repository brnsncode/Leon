using System;
using System.Collections.Generic;

namespace Leon.Models;

public partial class AppMetric
{
    public int EntryId { get; set; }

    public string AppName { get; set; } = null!;

    public TimeOnly AppUsedInSeconds { get; set; }

    public DateTime DateEntered { get; set; }
}
