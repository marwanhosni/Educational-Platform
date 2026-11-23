using Educational_Platform.Core.Common;
using Educational_Platform.Core.Dtos.Student;
using Educational_Platform.Core.Entities;
using Educational_Platform.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Educational_Platform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Student)]
public class MeController : ControllerBase
{
    private readonly IStudentService _studentService;

    public MeController(IStudentService studentService)
    {
        _studentService = studentService;
    }

   
    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<StudentProfileResponseDto>>> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<StudentProfileResponseDto>.Failure("UNAUTHORIZED", "غير مصرح لك بالوصول"));
        }

        var result = await _studentService.GetProfileAsync(userId);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    
    [HttpGet("group")]
    public async Task<ActionResult<ApiResponse<StudentMyGroupDto>>> GetMyGroup()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<StudentMyGroupDto>.Failure("UNAUTHORIZED", "غير مصرح لك بالوصول"));
        }

        var result = await _studentService.GetMyGroupAsync(userId);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}