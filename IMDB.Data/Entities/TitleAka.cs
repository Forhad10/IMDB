using System;
using System.Collections.Generic;

namespace IMDB.Data.Entities;

public partial class TitleAka
{
    public long AkaId { get; set; }

    public string TitleId { get; set; } = null!;

    public int? Ordering { get; set; }

    public string? LocalizedTitle { get; set; }

    public string? Region { get; set; }

    public string? Language { get; set; }

    public string? Types { get; set; }

    public string? Attributes { get; set; }

    public bool? IsOriginalTitle { get; set; }

    public virtual Title Title { get; set; } = null!;
}
