using Educational_Platform.Core.Common;
using Educational_Platform.Core.Dtos.Content;
using Educational_Platform.Core.Entities;
using Educational_Platform.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.Controllers;

[ApiController]
[Route("api/admin/courses")]
[Authorize(Roles = Roles.Admin)]
public class AdminCoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public AdminCoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

   
    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _courseService.CreateCourseAsync(dto);
        return Ok(result);
    }

  
    [HttpPost("chapters")]
    public async Task<IActionResult> CreateChapter([FromBody] CreateChapterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _courseService.CreateChapterAsync(dto);
        return Ok(result);
    }

  
    [HttpPost("lessons")]
    public async Task<IActionResult> CreateLesson([FromBody] CreateLessonDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _courseService.CreateLessonAsync(dto);
        return Ok(result);
    }

    [HttpPost("lessons/videos")]
    public async Task<IActionResult> AddVideo([FromBody] CreateVideoDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _courseService.AddVideoToLessonAsync(dto);
        return Ok(result);
    }

   
    [HttpPost("lessons/{id:int}/attachments")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddAttachment(int id, [FromForm] string title, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<string>.Failure("EMPTY_FILE", "يرجى اختيار ملف صالح"));

        if (!Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest(ApiResponse<string>.Failure("INVALID_FILE_TYPE", "الملفات المسموح بها بصيغة PDF فقط"));

        using var stream = file.OpenReadStream();
        var result = await _courseService.AddAttachmentToLessonAsync(id, title, stream, file.FileName);
        return Ok(result);
    }
}