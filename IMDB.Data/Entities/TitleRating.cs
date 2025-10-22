using System;
using System.Collections.Generic;

namespace IMDB.Data.Entities;

public partial class TitleRating
{
    public string TitleId { get; set; } = null!;

    public double? AverageRating { get; set; }

    public int? NumVotes { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public virtual Title Title { get; set; } = null!;
}
