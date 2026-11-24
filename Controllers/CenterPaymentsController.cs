using Educational_Platform.Core.Common;
using Educational_Platform.Core.Dtos.Payments;
using Educational_Platform.Core.Entities;
using Educational_Platform.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Educational_Platform.Controllers;

[ApiController]
[Route("api/secretary/center-payments")]
[Authorize(Roles = $"{Roles.Admin},{Roles.Secretary}")]
public class CenterPaymentsController : ControllerBase
{
    private readonly ICenterPaymentService _paymentService;

    public CenterPaymentsController(ICenterPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("record-cash")]
    public async Task<IActionResult> RecordCashPayment([FromBody] RecordCashPaymentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var secretaryId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _paymentService.RecordCashPaymentAsync(dto, secretaryId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

   
    [HttpGet("pending-receipts")]
    public async Task<IActionResult> GetPendingReceipts([FromQuery] int? groupId)
    {
        var result = await _paymentService.GetPendingReceiptsAsync(groupId);
        return Ok(result);
    }

    
    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> ApprovePayment(int id)
    {
        var secretaryId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _paymentService.ApprovePaymentAsync(id, secretaryId);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

  
    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> RejectPayment(int id, [FromBody] RejectPaymentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var secretaryId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _paymentService.RejectPaymentAsync(id, secretaryId, dto.Reason);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

  
    [HttpGet("unpaid")]
    public async Task<IActionResult> GetUnpaidStudents(
        [FromQuery] int month,
        [FromQuery] int year,
        [FromQuery] int? groupId = null)
    {
        if (month < 1 || month > 12)
            return BadRequest(ApiResponse<string>.Failure("INVALID_MONTH", "رقم الشهر يجب أن يكون بين 1 و 12"));

        var result = await _paymentService.GetUnpaidStudentsAsync(groupId, month, year);
        return Ok(result);
    }
}