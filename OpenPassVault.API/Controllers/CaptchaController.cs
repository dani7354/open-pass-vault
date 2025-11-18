using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenPassVault.Shared.Services.Interfaces;

namespace OpenPassVault.API.Controllers;

[Route("api/captcha")]
[ApiController]
[Authorize]
public class CaptchaController(ICaptchaService captachaService) : Controller
{
    [HttpGet("new")]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var captcha = await captachaService.GenerateCaptcha();
        return Ok(captcha);
    }
}