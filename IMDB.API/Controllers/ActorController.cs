using IMDB.Business.Services;
using IMDB.Business.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private readonly ActorService _actorService;

        public ActorController(ActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchActors([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query is required");
            }

            var request = new SearchActorsRequestDto
            {
                SearchQuery = query,
                Page = page,
                PageSize = pageSize
            };

            var result = await _actorService.SearchActorsAsync(request);

            var response = new
            {
                page = result.Page,
                pageSize = result.PageSize,
                total = result.TotalCount,
                searchQuery = result.SearchQuery,
                data = result.Data,
                links = new
                {
                    self = Url.Action(nameof(SearchActors), new { query, page, pageSize }),
                    next = Url.Action(nameof(SearchActors), new { query, page = page + 1, pageSize }),
                    prev = page > 1 ? Url.Action(nameof(SearchActors), new { query, page = page - 1, pageSize }) : null
                }
            };

            return Ok(response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllActors([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            {
                var request = new GetAllActorsRequestDto
                {
                    Page = page,
                    PageSize = pageSize
                };

                var result = await _actorService.GetAllActorsAsync(request);

                var response = new
                {
                    page = result.Page,
                    pageSize = result.PageSize,
                    total = result.TotalCount,
                    searchQuery = "All Actors",
                    data = result.Data,
                    links = new
                    {
                        self = Url.Action(nameof(GetAllActors), new { page, pageSize }),
                        next = Url.Action(nameof(GetAllActors), new { page = page + 1, pageSize }),
                        prev = page > 1 ? Url.Action(nameof(GetAllActors), new { page = page - 1, pageSize }) : null
                    }
                };

                return Ok(response);
            }
            [HttpGet("coplayers/{actorId}")]
            public async Task<IActionResult> GetCoPlayers(string actorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            {
                if (string.IsNullOrWhiteSpace(actorId))
                {
                    return BadRequest("Actor ID is required");
                }

                var request = new CoPlayersRequestDto
                {
                    ActorId = actorId,
                    Page = page,
                    PageSize = pageSize
                };

                var result = await _actorService.GetCoPlayersAsync(request);

                var response = new
                {
                    page = result.Page,
                    pageSize = result.PageSize,
                    total = result.TotalCount,
                    actorId = result.ActorId,
                    searchQuery = result.SearchQuery,
                    data = result.Data,
                    links = new
                    {
                        self = Url.Action(nameof(GetCoPlayers), new { actorId, page, pageSize }),
                        next = Url.Action(nameof(GetCoPlayers), new { actorId, page = page + 1, pageSize }),
                        prev = page > 1 ? Url.Action(nameof(GetCoPlayers), new { actorId, page = page - 1, pageSize }) : null
                    }
                };

                return Ok(response);
            }

            [HttpGet("popular/{titleId}")]
            public async Task<IActionResult> GetPopularActors(string titleId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            {
                if (string.IsNullOrWhiteSpace(titleId))
                {
                    return BadRequest("Title ID is required");
                }

                var request = new PopularActorsRequestDto
                {
                    TitleId = titleId,
                    Page = page,
                    PageSize = pageSize
                };

                var result = await _actorService.GetPopularActorsAsync(request);

                var response = new
                {
                    page = result.Page,
                    pageSize = result.PageSize,
                    total = result.TotalCount,
                    titleId = result.TitleId,
                    searchQuery = result.SearchQuery,
                    data = result.Data,
                    links = new
                    {
                        self = Url.Action(nameof(GetPopularActors), new { titleId, page, pageSize }),
                        next = Url.Action(nameof(GetPopularActors), new { titleId, page = page + 1, pageSize }),
                        prev = page > 1 ? Url.Action(nameof(GetPopularActors), new { titleId, page = page - 1, pageSize }) : null
                    }
                };

                return Ok(response);
            }
        }
    }

