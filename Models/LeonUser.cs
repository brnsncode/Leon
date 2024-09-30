using System;
using System.Collections.Generic;

namespace Leon.Models;

public partial class LeonUser
{
    public Guid LeonGuid { get; set; }

    public string GivenName { get; set; } = null!;

    public string? Surname { get; set; }

    public Guid? Adguid { get; set; }

    public string? SamAccountName { get; set; }

    public string? DisplayName { get; set; }

    public bool? ShowDetails { get; set; }
}
