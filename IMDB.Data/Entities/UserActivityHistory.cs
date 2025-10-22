using System;
using System.Collections.Generic;

namespace IMDB.Data.Entities;

public partial class UserActivityHistory
{
    public long HistoryId { get; set; }

    public Guid UserId { get; set; }

    public string? ActionType { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }
}
