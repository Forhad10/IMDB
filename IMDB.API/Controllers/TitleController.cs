using IMDB.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TitleController : ControllerBase
    {
        private readonly TitleService _service;
        public TitleController(TitleService service) { _service = service; }

        [HttpGet]

        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllAsync(page, pageSize);

            var response = new
            {
                page = result.Page,
                pageSize = result.PageSize,
                total = result.TotalCount,
                data = result.Data,
                links = new
                {
                    self = Url.Action(nameof(GetAll), new { page, pageSize }),
                    next = Url.Action(nameof(GetAll), new { page = page + 1, pageSize }),
                    prev = page > 1 ? Url.Action(nameof(GetAll), new { page = page - 1, pageSize }) : null
                }
            };

            return Ok(response);
        }


    }
}
