using System;
using System.Collections.Generic;

namespace IMDB.Data.Entities;

public partial class TitlePrincipal
{
    public long PrincipalId { get; set; }

    public string TitleId { get; set; } = null!;

    public int? Ordering { get; set; }

    public string ActorId { get; set; } = null!;

    public string? Category { get; set; }

    public string? Job { get; set; }

    public string? Characters { get; set; }
}
