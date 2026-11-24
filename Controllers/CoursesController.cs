using Educational_Platform.Core.Common;
using Educational_Platform.Core.Dtos.Content;
using Educational_Platform.Core.Entities;
using Educational_Platform.Core.EnumGrade;
using Educational_Platform.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Educational_Platform.Controllers;

[ApiController]
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourses([FromQuery] Grade? grade)
    {
        var studentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _courseService.GetCoursesAsync(grade, studentUserId);
        return Ok(result);
    }

  
    [HttpGet("{id:int}/syllabus")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCourseSyllabus(int id)
    {
        var studentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _courseService.GetCourseSyllabusAsync(id, studentUserId);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    
    [HttpPost("lessons/{id:int}/watch")]
    [Authorize(Roles = Roles.Student)]
    public async Task<IActionResult> WatchLesson(int id)
    {
        var studentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _courseService.WatchLessonAsync(id, studentUserId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}