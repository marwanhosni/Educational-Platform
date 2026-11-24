using Educational_Platform.Core.Common;
using Educational_Platform.Core.Dtos.PaymentGateway;
using Educational_Platform.Core.Entities;
using Educational_Platform.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Educational_Platform.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentGatewayService _paymentService;

    public PaymentsController(IPaymentGatewayService paymentService)
    {
        _paymentService = paymentService;
    }

   
    [HttpPost("checkout")]
    [Authorize(Roles = Roles.Student)]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto dto)
    {
        var studentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentUserId))
            return Unauthorized(ApiResponse<object>.Failure("UNAUTHORIZED", "غير مصرح لك"));

        var result = await _paymentService.InitiateCheckoutAsync(dto, studentUserId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    
    [HttpPost("paymob-webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> PaymobWebhook([FromBody] PaymobWebhookPayload payload)
    {
        var hmac = Request.Headers["hmac"].FirstOrDefault() ?? Request.Query["hmac"].FirstOrDefault();
        var result = await _paymentService.ProcessPaymobWebhookAsync(payload, hmac);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    
    [HttpPost("simulate-webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> SimulateWebhook([FromBody] SimulatePaymentWebhookDto dto)
    {
        var result = await _paymentService.SimulatePaymentSuccessAsync(dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}