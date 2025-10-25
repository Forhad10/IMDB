using System.Collections.Generic;

namespace IMDB.Business.DTOs
{
    public class PaginatedTitleResponseDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<TitleDto> Data { get; set; } = new List<TitleDto>();
    }


    public class TitleDto
    {
        public string TitleId { get; set; } = string.Empty;
        public string? PrimaryTitle { get; set; }
        public string? TitleType { get; set; }
        public string? Genres { get; set; }
        public string? StartYear { get; set; }
        public decimal AverageRating { get; set; }
        public int NumVotes { get; set; }
    }
}
