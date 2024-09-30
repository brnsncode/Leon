using System;
using System.Collections.Generic;

namespace Leon.Models;

public partial class AssignedTaskNote
{
    public int NoteId { get; set; }

    public int TaskId { get; set; }

    public int ResourceId { get; set; }

    public string? NoteText { get; set; }

    public DateTime CreationDate { get; set; }

    public virtual Resource Resource { get; set; } = null!;

    public virtual AssignedTask Task { get; set; } = null!;
}
