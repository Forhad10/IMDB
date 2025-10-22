using System;
using System.Collections.Generic;

namespace IMDB.Data.Entities;

public partial class Title
{
    public string TitleId { get; set; } = null!;

    public string? PrimaryTitle { get; set; }

    public string? OriginalTitle { get; set; }

    public string? TitleType { get; set; }

    public string? StartYear { get; set; }

    public string? EndYear { get; set; }

    public string? RuntimeMinutes { get; set; }

    public string? Genres { get; set; }

    public string? Plot { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<TitleAka> TitleAkas { get; set; } = new List<TitleAka>();

    public virtual TitleRating? TitleRating { get; set; }

    public virtual ICollection<WordIndexTitle> WordIndexTitles { get; set; } = new List<WordIndexTitle>();
}
