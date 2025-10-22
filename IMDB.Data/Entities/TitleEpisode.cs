using System;
using System.Collections.Generic;

namespace IMDB.Data.Entities;

public partial class TitleEpisode
{
    public string EpisodeId { get; set; } = null!;

    public string? ParentTitleId { get; set; }

    public int? SeasonNumber { get; set; }

    public int? EpisodeNumber { get; set; }
}
