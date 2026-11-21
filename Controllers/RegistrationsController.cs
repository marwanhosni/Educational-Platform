using System.Security.Claims;
using Educational_Platform.Core.Common;
using Educational_Platform.Core.Dtos.Registrations;
using Educational_Platform.Core.Entities;
using Educational_Platform.Core.EnumGrade;
using Educational_Platform.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Secretary}")] 
public class RegistrationsController : ControllerBase
{
    private readonly IRegistrationService _registrationService;

    public RegistrationsController(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

   
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingRegistrations([FromQuery] Grade? grade, [FromQuery] StudentType? type)
    {
        var result = await _registrationService.GetPendingRegistrationsAsync(grade, type);
        return Ok(result);
    }

  
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRegistrationDetails(int id)
    {
        var result = await _registrationService.GetRegistrationDetailsAsync(id);
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> ApproveRegistration(int id, [FromBody] ApproveStudentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var isAdmin = User.IsInRole(Roles.Admin);

        var result = await _registrationService.ApproveRegistrationAsync(id, dto, currentUserId, isAdmin);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

   
    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> RejectRegistration(int id, [FromBody] RejectStudentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        var result = await _registrationService.RejectRegistrationAsync(id, dto, currentUserId);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpGet("summary")]
    public async Task<IActionResult> GetStaffSummary()
    {
        var result = await _registrationService.GetStaffSummaryAsync();
        return Ok(result);
    }
}