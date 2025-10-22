using System;
using System.Collections.Generic;

namespace IMDB.Data.Entities;

public partial class ActorsRating
{
    public string NameId { get; set; } = null!;

    public double? WeightedRating { get; set; }

    public int? TotalVotes { get; set; }

    public DateTime? LastUpdatedAt { get; set; }
}
