using System.Collections.Generic;

namespace IMDB.Business.DTOs
{
    public class BasicMovieSearchRequestDto
    {
        public string SearchQuery { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? UserId { get; set; } = null;
    }

    public class StructuredMovieSearchRequestDto
    {
        public string? Title { get; set; }
        public string? Plot { get; set; }
        public string? Characters { get; set; }
        public string? Person { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? UserId { get; set; } = null;
    }

    public class SimilarMoviesRequestDto
    {
        public string TitleId { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid? UserId { get; set; } = null;
    }

    public class MovieSearchResultDto
    {
        public string TitleId { get; set; } = string.Empty;
        public string? PrimaryTitle { get; set; }
        public string? TitleType { get; set; }
        public string? Genres { get; set; }
        public decimal AverageRating { get; set; }
        public int NumVotes { get; set; }
        
        // User-specific data
        public int? BookmarkId { get; set; }
        public Guid? UserIdBookmark { get; set; }
        public string? RatingHistoryId { get; set; }
        public Guid? UserIdRating { get; set; }
        public decimal? UserRating { get; set; }
    }

    public class MovieSearchResponseDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string? SearchQuery { get; set; }
        public string? TitleFilter { get; set; }
        public string? PlotFilter { get; set; }
        public string? CharactersFilter { get; set; }
        public string? PersonFilter { get; set; }
        public string? BaseTitleId { get; set; }
        public IEnumerable<MovieSearchResultDto> Data { get; set; } = new List<MovieSearchResultDto>();
    }
}
