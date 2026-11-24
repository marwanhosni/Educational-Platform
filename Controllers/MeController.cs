using Educational_Platform.Core.Common;
using Educational_Platform.Core.Dtos.Student;
using Educational_Platform.Core.Entities;
using Educational_Platform.Core.EnumGrade;
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
    private readonly ICenterPaymentService _paymentService;

    public MeController(IStudentService studentService, ICenterPaymentService paymentService)
    {
        _studentService = studentService;
        _paymentService = paymentService;
    }

   
    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponse<StudentProfileResponseDto>>> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse<StudentProfileResponseDto>.Failure("UNAUTHORIZED", "غير مصرح لك"));

        var result = await _studentService.GetProfileAsync(userId);
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }


    [HttpGet("group")]
    public async Task<ActionResult<ApiResponse<StudentMyGroupDto>>> GetMyGroup()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse<StudentMyGroupDto>.Failure("UNAUTHORIZED", "غير مصرح لك"));

        var result = await _studentService.GetMyGroupAsync(userId);
        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    [HttpGet("payments")]
    public async Task<IActionResult> GetMyPayments()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _paymentService.GetMyPaymentsHistoryAsync(userId);
        return Ok(result);
    }

    [HttpPost("payments/upload-receipt")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadReceipt(
        [FromForm] int month,
        [FromForm] int year,
        [FromForm] PaymentMethod method,
        [FromForm] string? transactionRef,
        IFormFile receiptFile)
    {
        if (month < 1 || month > 12)
            return BadRequest(ApiResponse<string>.Failure("INVALID_MONTH", "رقم الشهر غير صالح"));

        if (receiptFile == null || receiptFile.Length == 0)
            return BadRequest(ApiResponse<string>.Failure("EMPTY_FILE", "يرجى اختيار صورة الإيصال"));

        const long maxFileSize = 5 * 1024 * 1024;
        if (receiptFile.Length > maxFileSize)
            return BadRequest(ApiResponse<string>.Failure("FILE_TOO_LARGE", "حجم صورة الإيصال لا يجب أن يتجاوز 5 ميجابايت"));

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(receiptFile.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest(ApiResponse<string>.Failure("INVALID_EXTENSION", "امتداد الصورة غير مسموح به"));

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        using var stream = receiptFile.OpenReadStream();

        var result = await _paymentService.SubmitReceiptPaymentAsync(
            userId, month, year, method, stream, receiptFile.FileName, transactionRef);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}