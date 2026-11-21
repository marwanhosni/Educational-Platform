using Educational_Platform.Core.Common;
using Educational_Platform.Core.Dtos.Secretaries;
using Educational_Platform.Core.Entities;
using Educational_Platform.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.Controllers;

[ApiController]
[Route("api/admin/secretaries")]
[Authorize(Roles = Roles.Admin)] 
public class SecretariesController : ControllerBase
{
    private readonly ISecretaryService _secretaryService;

    public SecretariesController(ISecretaryService secretaryService)
    {
        _secretaryService = secretaryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSecretaries()
    {
        var result = await _secretaryService.GetAllSecretariesAsync();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSecretary([FromBody] CreateSecretaryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _secretaryService.CreateSecretaryAsync(dto);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPatch("{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        var result = await _secretaryService.ToggleSecretaryStatusAsync(id);
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}