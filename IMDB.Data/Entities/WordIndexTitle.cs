using System;
using System.Collections.Generic;

namespace IMDB.Data.Entities;

public partial class WordIndexTitle
{
    public long WiId { get; set; }

    public long WordId { get; set; }

    public string TitleId { get; set; } = null!;

    public string? SourceField { get; set; }

    public int? TermFreq { get; set; }

    public double? Weight { get; set; }

    public virtual Title Title { get; set; } = null!;

    public virtual WordIndex Word { get; set; } = null!;
}
