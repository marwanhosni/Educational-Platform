using Educational_Platform.Core.Dtos.Exams;
using Educational_Platform.Core.Entities;
using Educational_Platform.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Educational_Platform.Controllers;

[ApiController]
[Route("api/exams")]
[Authorize(Roles = Roles.Student)]
public class ExamsController : ControllerBase
{
    private readonly IExamService _examService;

    public ExamsController(IExamService examService)
    {
        _examService = examService;
    }

   
    [HttpGet("{id:int}/take")]
    public async Task<IActionResult> GetExamForTaking(int id)
    {
        var studentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _examService.GetExamForTakingAsync(id, studentUserId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpPost("submit")]
    public async Task<IActionResult> SubmitExam([FromBody] SubmitExamDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var studentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _examService.SubmitExamAsync(dto, studentUserId);

        return Ok(result);
    }
}