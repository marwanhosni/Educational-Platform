using Educational_Platform.Core.Dtos.Exams;
using Educational_Platform.Core.Entities;
using Educational_Platform.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Educational_Platform.Controllers;

[ApiController]
[Route("api/admin/exams")]
[Authorize(Roles = Roles.Admin)]
public class AdminExamsController : ControllerBase
{
    private readonly IExamService _examService;

    public AdminExamsController(IExamService examService)
    {
        _examService = examService;
    }

  
    [HttpPost]
    public async Task<IActionResult> CreateExam([FromBody] CreateExamDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _examService.CreateExamAsync(dto);
        return Ok(result);
    }


    [HttpPost("questions")]
    public async Task<IActionResult> AddQuestion([FromBody] CreateQuestionDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _examService.AddQuestionAsync(dto);
        return Ok(result);
    }
}