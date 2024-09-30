using System;
using System.Collections.Generic;

namespace Leon.Models;

public partial class Resource
{
    public int ResourceId { get; set; }

    public string? EmployeeName { get; set; }

    public bool? ShowDetails { get; set; }

    public Guid? Adguid { get; set; }

    public virtual ICollection<AssignedTaskNote> AssignedTaskNotes { get; set; } = new List<AssignedTaskNote>();

    public virtual ICollection<AssignedTask> AssignedTasks { get; set; } = new List<AssignedTask>();

    public virtual ICollection<Operation> Operations { get; set; } = new List<Operation>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
