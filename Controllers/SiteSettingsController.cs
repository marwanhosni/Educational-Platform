using Educational_Platform.Core.Common;
using Educational_Platform.Core.Dtos.Site;
using Educational_Platform.Core.Entities;
using Educational_Platform.Core.Interface;
using Educational_Platform.Repository.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Educational_Platform.Controllers;

[ApiController]
[Route("api/admin/site-settings")]
[Authorize(Roles = Roles.Admin)]
public class SiteSettingsController : ControllerBase
{
    private readonly ISiteSettingService _siteSettingService;
    private readonly IFileService _fileService;
    private readonly AppDbContext _context;
    private readonly IAuditService _auditService;

    public SiteSettingsController(
        ISiteSettingService siteSettingService,
        IFileService fileService,
        AppDbContext context,
        IAuditService auditService)
    {
        _siteSettingService = siteSettingService;
        _fileService = fileService;
        _context = context;
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<SiteInfoResponseDto>>> GetSettings()
    {
        var result = await _siteSettingService.GetSiteInfoAsync();
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse<SiteInfoResponseDto>>> UpdateSettings([FromBody] UpdateSiteSettingsDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _siteSettingService.UpdateSiteSettingsAsync(dto);
        return Ok(result);
    }

    [HttpPost("upload-photo")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadTeacherPhoto(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<string>.Failure("EMPTY_FILE", "يرجى اختيار ملف صورة صالح"));
        }

        const long maxFileSize = 5 * 1024 * 1024;
        if (file.Length > maxFileSize)
        {
            return BadRequest(ApiResponse<string>.Failure("FILE_TOO_LARGE", "حجم الصورة لا يجب أن يتجاوز 5 ميجابايت"));
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(ApiResponse<string>.Failure("INVALID_EXTENSION", "امتداد الصورة غير مسموح به. الصيغ المتاحة: jpg, jpeg, png, webp"));
        }

        var currentSettings = await _context.SiteSettings.FirstOrDefaultAsync();
        if (currentSettings != null && !string.IsNullOrEmpty(currentSettings.TeacherPhotoUrl))
        {
            var uri = new Uri(currentSettings.TeacherPhotoUrl, UriKind.RelativeOrAbsolute);
            var relativePath = uri.IsAbsoluteUri ? uri.AbsolutePath : currentSettings.TeacherPhotoUrl;
            _fileService.DeleteFile(relativePath);
        }

        using var stream = file.OpenReadStream();
        var newPath = await _fileService.SaveFileAsync(stream, file.FileName, "uploads/teacher");
        var fullUrl = $"{Request.Scheme}://{Request.Host}{newPath}";

        if (currentSettings == null)
        {
            currentSettings = new SiteSettings();
            _context.SiteSettings.Add(currentSettings);
        }

        currentSettings.TeacherPhotoUrl = fullUrl;
        await _context.SaveChangesAsync();

        await _auditService.LogAsync("UploadPhoto", "SiteSettings", currentSettings.Id.ToString(), "تم رفع صورة جديدة للمعلم وتحديث المنصة");

        return Ok(ApiResponse<object>.SuccessResult(new
        {
            PhotoUrl = fullUrl,
            RelativePath = newPath
        }, "تم رفع صورة المعلم وتحديث المنصة بنجاح"));
    }

    [HttpDelete("photo")]
    public async Task<IActionResult> DeleteTeacherPhoto()
    {
        var settings = await _context.SiteSettings.FirstOrDefaultAsync();
        if (settings == null || string.IsNullOrEmpty(settings.TeacherPhotoUrl))
        {
            return NotFound(ApiResponse<string>.Failure("NOT_FOUND", "لا توجد صورة مسجلة للمعلم"));
        }

        var uri = new Uri(settings.TeacherPhotoUrl, UriKind.RelativeOrAbsolute);
        var relativePath = uri.IsAbsoluteUri ? uri.AbsolutePath : settings.TeacherPhotoUrl;
        _fileService.DeleteFile(relativePath);

        settings.TeacherPhotoUrl = null;
        await _context.SaveChangesAsync();

        await _auditService.LogAsync("DeletePhoto", "SiteSettings", settings.Id.ToString(), "تم حذف صورة المعلم نهائياً");

        return Ok(ApiResponse<string>.SuccessResult(string.Empty, "تم حذف صورة المعلم بنجاح"));
    }
}