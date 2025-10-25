using IMDB.Business.Services;
using IMDB.Business.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieSearchController : ControllerBase
    {
        private readonly MovieSearchService _searchService;

        public MovieSearchController(MovieSearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("basic")]
        public async Task<IActionResult> BasicSearch([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query is required");
            }

            var request = new BasicMovieSearchRequestDto
            {
                SearchQuery = query,
                Page = page,
                PageSize = pageSize
            };

            var result = await _searchService.BasicSearchAsync(request);

            var response = new
            {
                page = result.Page,
                pageSize = result.PageSize,
                total = result.TotalCount,
                searchQuery = result.SearchQuery,
                data = result.Data,
                links = new
                {
                    self = Url.Action(nameof(BasicSearch), new { query, page, pageSize }),
                    next = Url.Action(nameof(BasicSearch), new { query, page = page + 1, pageSize }),
                    prev = page > 1 ? Url.Action(nameof(BasicSearch), new { query, page = page - 1, pageSize }) : null
                }
            };

            return Ok(response);
        }

        [HttpGet("structured")]
        public async Task<IActionResult> StructuredSearch(
            [FromQuery] string? title,
            [FromQuery] string? plot,
            [FromQuery] string? characters,
            [FromQuery] string? person,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // At least one search parameter is required
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(plot) &&
                string.IsNullOrWhiteSpace(characters) && string.IsNullOrWhiteSpace(person))
            {
                return BadRequest("At least one search parameter is required (title, plot, characters, or person)");
            }

            var request = new StructuredMovieSearchRequestDto
            {
                Title = title,
                Plot = plot,
                Characters = characters,
                Person = person,
                Page = page,
                PageSize = pageSize
            };

            var result = await _searchService.StructuredSearchAsync(request);

            var response = new
            {
                page = result.Page,
                pageSize = result.PageSize,
                total = result.TotalCount,
                searchQuery = result.SearchQuery,
                titleFilter = result.TitleFilter,
                plotFilter = result.PlotFilter,
                charactersFilter = result.CharactersFilter,
                personFilter = result.PersonFilter,
                data = result.Data,
                links = new
                {
                    self = Url.Action(nameof(StructuredSearch), new { title, plot, characters, person, page, pageSize }),
                    next = Url.Action(nameof(StructuredSearch), new { title, plot, characters, person, page = page + 1, pageSize }),
                    prev = page > 1 ? Url.Action(nameof(StructuredSearch), new { title, plot, characters, person, page = page - 1, pageSize }) : null
                }
            };

            return Ok(response);
        }

        [HttpGet("similar/{titleId}")]
        public async Task<IActionResult> GetSimilarMovies(string titleId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(titleId))
            {
                return BadRequest("Title ID is required");
            }

            var request = new SimilarMoviesRequestDto
            {
                TitleId = titleId,
                Page = page,
                PageSize = pageSize
            };

            var result = await _searchService.GetSimilarMoviesAsync(request);

            var response = new
            {
                page = result.Page,
                pageSize = result.PageSize,
                total = result.TotalCount,
                baseTitleId = result.BaseTitleId,
                searchQuery = result.SearchQuery,
                data = result.Data,
                links = new
                {
                    self = Url.Action(nameof(GetSimilarMovies), new { titleId, page, pageSize }),
                    next = Url.Action(nameof(GetSimilarMovies), new { titleId, page = page + 1, pageSize }),
                    prev = page > 1 ? Url.Action(nameof(GetSimilarMovies), new { titleId, page = page - 1, pageSize }) : null
                }
            };

            return Ok(response);
        }
    }
}
