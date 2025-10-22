using System;

namespace IMDB.Business.DTOs
{
    public class TitleDto
    {
        public string TitleId { get; set; } = string.Empty;
        public string? PrimaryTitle { get; set; }
        public string? TitleType { get; set; }
        public string? Genres { get; set; }
        public decimal AverageRating { get; set; }
        public int NumVotes { get; set; }
    }
}
