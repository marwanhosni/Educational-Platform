using Educational_Platform.Core.Common;
using Educational_Platform.Core.Dtos.Site;
using Educational_Platform.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.Controllers;

[ApiController]
[Route("api/public")]
[AllowAnonymous]
public class PublicController : ControllerBase
{
    private readonly ISiteSettingService _siteSettingService;

    public PublicController(ISiteSettingService siteSettingService)
    {
        _siteSettingService = siteSettingService;
    }

    
    [HttpGet("site-info")]
    public async Task<ActionResult<ApiResponse<SiteInfoResponseDto>>> GetSiteInfo()
    {
        var result = await _siteSettingService.GetSiteInfoAsync();
        return Ok(result);
    }

    [HttpGet("grades")]
    public async Task<ActionResult<ApiResponse<List<GradeResponseDto>>>> GetGrades()
    {
        var result = await _siteSettingService.GetGradesAsync();
        return Ok(result);
    }
}