using System;
using System.Collections.Generic;

namespace IMDB.Data.Entities;

public partial class UserRatingHistory
{
    public long RatingHistoryId { get; set; }

    public Guid UserId { get; set; }

    public string TitleId { get; set; } = null!;

    public short? Rating { get; set; }

    public DateTime? RatedAt { get; set; }
}
