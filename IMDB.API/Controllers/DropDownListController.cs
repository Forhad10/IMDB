using IMDB.Business.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DropDownListController : ControllerBase
{
    private readonly DropdownListService _dropdownService;

    public DropDownListController(DropdownListService dropdownService)
    {
        _dropdownService = dropdownService;
    }

    [HttpGet("actorList")]
    public async Task<IActionResult> GetActors()
    {
        var actors = await _dropdownService.GetActorsForDropdownAsync();
        return Ok(actors);
    }
}