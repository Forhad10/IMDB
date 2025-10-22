using System;
using System.Collections.Generic;

namespace IMDB.Data.Entities;

public partial class UserBookmark
{
    public long BookmarkId { get; set; }

    public Guid UserId { get; set; }

    public string? EntityType { get; set; }

    public string? EntityId { get; set; }

    public DateTime? BookmarkedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
