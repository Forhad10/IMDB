using System;
using System.Collections.Generic;

namespace IMDB.Data.Entities;

public partial class UserNote
{
    public long NoteId { get; set; }

    public Guid UserId { get; set; }

    public string? EntityType { get; set; }

    public string? EntityId { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
