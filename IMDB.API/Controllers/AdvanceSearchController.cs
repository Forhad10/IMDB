using IMDB.Business.DTOs;
using IMDB.Business.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMDB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvanceSearchController : ControllerBase
    {
        private readonly AdvanceSearchService _service;

        public AdvanceSearchController(AdvanceSearchService service)
        {
            _service = service;
        }

        [HttpGet("person-words/{actorId}")]
        public async Task<IActionResult> GetPersonWords(string actorId, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(actorId))
            {
                return BadRequest("Actor ID is required");
            }

            var data = await _service.GetPersonWordsAsync(actorId, limit);

            var response = new
            {
                actorId,
                limit,
                data
            };

            return Ok(response);
        }


        [HttpPost("exact-match-titles")]
        public async Task<IActionResult> GetExactMatchTitles([FromBody] ExactorBestMatchSearchRequestDto request)
        {
            var result = await _service.GetExactMatchTitlesAsync(request);

            var response = new
            {
                page = result.Page,
                pageSize = result.PageSize,
                total = result.TotalCount,
                data = result.Data,
                links = new
                {
                    self = Url.Action(nameof(GetExactMatchTitles), new
                    {
                        page = request.Page,
                        pageSize = request.PageSize
                    }),
                    next = result.TotalCount > request.Page * request.PageSize
                        ? Url.Action(nameof(GetExactMatchTitles), new
                        {
                            page = request.Page + 1,
                            pageSize = request.PageSize
                        })
                        : null,
                    prev = request.Page > 1
                        ? Url.Action(nameof(GetExactMatchTitles), new
                        {
                            page = request.Page - 1,
                            pageSize = request.PageSize
                        })
                        : null
                }
            };

            return Ok(response);
        }



        [HttpPost("best-match-titles")]
        public async Task<IActionResult> GetBestMatchTitles([FromBody] ExactorBestMatchSearchRequestDto request)
        {
            var result = await _service.GetBestMatchTitlesAsync(request);

            var response = new
            {
                page = result.Page,
                pageSize = result.PageSize,
                total = result.TotalCount,
                data = result.Data,
                links = new
                {
                    self = Url.Action(nameof(GetExactMatchTitles), new
                    {
                        page = request.Page,
                        pageSize = request.PageSize
                    }),
                    next = result.TotalCount > request.Page * request.PageSize
                        ? Url.Action(nameof(GetExactMatchTitles), new
                        {
                            page = request.Page + 1,
                            pageSize = request.PageSize
                        })
                        : null,
                    prev = request.Page > 1
                        ? Url.Action(nameof(GetExactMatchTitles), new
                        {
                            page = request.Page - 1,
                            pageSize = request.PageSize
                        })
                        : null
                }
            };

            return Ok(response);
        }

      

        [HttpPost("keyword-word-list")]
        public async Task<IActionResult> GetKeywordWordList([FromBody] IEnumerable<string> keywords)
        {
            if (keywords == null || !keywords.Any())
            {
                return BadRequest("Keywords array is required");
            }

            var data = await _service.GetKeywordWordListAsync(keywords.ToArray());

            var response = new
            {
                keywords,
                data
            };

            return Ok(response);
        }
    }
}