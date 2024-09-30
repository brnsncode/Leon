using System;
using System.Collections.Generic;

namespace Leon.Models;

public partial class AssignedTask
{
    public int TaskId { get; set; }

    public string? TaskName { get; set; }

    public int? ProjectId { get; set; }

    public int? OperationId { get; set; }

    public int? ResourceId { get; set; }

    public int? CapacityPercentage { get; set; }

    public string? Status { get; set; }

    public bool? ShowNewTaskButton { get; set; }

    public bool? IsReminder { get; set; }

    public DateTime? DateEntered { get; set; }

    public bool? LinkedToPlanner { get; set; }

    public DateTime? LastUpdated { get; set; }

    public bool? Ongoing { get; set; }

    public DateTime? SetReminderDate { get; set; }

    public bool? LinkedToOneList { get; set; }

    public virtual ICollection<AssignedTaskNote> AssignedTaskNotes { get; set; } = new List<AssignedTaskNote>();

    public virtual Resource? Resource { get; set; }
}
