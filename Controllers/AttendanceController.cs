using Educational_Platform.Core.Common;
using Educational_Platform.Core.Dtos.Attendance;
using Educational_Platform.Core.Entities;
using Educational_Platform.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Educational_Platform.Core.Dtos.Attendance.ScanBarcodeDto;

namespace Educational_Platform.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = $"{Roles.Admin},{Roles.Secretary}")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    [HttpPost("sessions")]
    public async Task<IActionResult> CreateSession([FromBody] CreateAttendanceSessionDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _attendanceService.CreateSessionAsync(dto, userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("groups/{groupId}/sessions")]
    public async Task<IActionResult> GetSessionsByGroup(int groupId)
    {
        var result = await _attendanceService.GetSessionsByGroupAsync(groupId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("sessions/{sessionId}/sheet")]
    public async Task<IActionResult> GetSessionSheet(int sessionId)
    {
        var result = await _attendanceService.GetSessionSheetAsync(sessionId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("scan")]
    public async Task<IActionResult> ScanBarcode([FromBody] ScanBarcodeDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _attendanceService.ScanBarcodeAsync(dto, userId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("sessions/bulk")]
    public async Task<IActionResult> UpdateBulkAttendance([FromBody] BulkAttendanceUpdateDto dto)
    {
        var result = await _attendanceService.UpdateBulkAttendanceAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("sessions/{sessionId}/absentees")]
    public async Task<IActionResult> GetSessionAbsentees(int sessionId)
    {
        var result = await _attendanceService.GetSessionAbsenteesAsync(sessionId);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}