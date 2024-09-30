using System;
using System.Collections.Generic;

namespace Leon.Models;

public partial class KanbanColumn
{
    public int SectionId { get; set; }

    public string SectionName { get; set; } = null!;

    public Guid SectionGuid { get; set; }
}
