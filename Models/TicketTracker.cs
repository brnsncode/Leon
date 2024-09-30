using System;
using System.Collections.Generic;

namespace Leon.Models;

public partial class TicketTracker
{
    public int TicketEntryId { get; set; }

    public string? TickerNumberNotes { get; set; }

    public DateTime? DateEntered { get; set; }
}
