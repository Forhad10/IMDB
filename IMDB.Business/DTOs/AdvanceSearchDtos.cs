using System.Collections.Generic;

namespace IMDB.Business.DTOs
{
    public class SearchActorsRequestDto
    {
        public string SearchQuery { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class CoPlayersRequestDto
    {
        public string ActorId { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetAllActorsRequestDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PopularActorsRequestDto
    {
        public string TitleId { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ActorDto
    {
        public string NameId { get; set; } = string.Empty;
        public string? PrimaryName { get; set; }
        public string? BirthYear { get; set; }
        public string? DeathYear { get; set; }
        public string? PrimaryProfession { get; set; }
        public double? WeightedRating { get; set; }
        public int? TotalVotes { get; set; }
        public int? Frequency { get; set; }
    }

    public class ActorSearchResponseDto
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string? SearchQuery { get; set; }
        public string? ActorId { get; set; }
        public string? TitleId { get; set; }
        public IEnumerable<ActorDto> Data { get; set; } = new List<ActorDto>();
    }


    public class ExactorBestMatchSearchRequestDto
{
    public string[] Keywords { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? UserId { get; set; }
}
}
