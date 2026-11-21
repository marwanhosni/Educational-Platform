using Educational_Platform.Core.Common;
using Educational_Platform.Core.Dtos.Auth;
using Educational_Platform.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Educational_Platform.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
   
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterStudentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _authService.RegisterStudentAsync(dto);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }
  
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(dto);
        if (!result.Success)
        {
            if (result.Code == "ACCOUNT_PENDING" ||
                result.Code == "ACCOUNT_REJECTED" ||
                result.Code == "ACCOUNT_SUSPENDED")
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            if (result.Code == "ACCOUNT_LOCKED")
            {
                return StatusCode(StatusCodes.Status423Locked, result);
            }
            return Unauthorized(result);
        }
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }
   
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RevokeTokenAsync(dto.RefreshToken);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }
    

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var name = User.FindFirstValue(ClaimTypes.Name);
        var role = User.FindFirstValue(ClaimTypes.Role);
        return Ok(ApiResponse<object>.SuccessResult(new
        {
            UserId = userId,
            Email = email,
            FullName = name,
            Role = role
        }, "تم جلب بيانات المستخدم بنجاح"));
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _authService.ForgotPasswordAsync(dto);
        return Ok(result);
    }
   
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var result = await _authService.ResetPasswordAsync(dto);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }
}
