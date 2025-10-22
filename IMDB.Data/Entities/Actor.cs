using System;
using System.Collections.Generic;

namespace IMDB.Data.Entities;

public partial class Actor
{
    public string NameId { get; set; } = null!;

    public string? PrimaryName { get; set; }

    public string? BirthYear { get; set; }

    public string? DeathYear { get; set; }

    public string? PrimaryProfession { get; set; }

    public DateTime? CreatedAt { get; set; }
}
