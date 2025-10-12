using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using OpenPassVault.API.Data.Entity;
using OpenPassVault.API.Services.Interfaces;
using OpenPassVault.Shared.DTO;

namespace OpenPassVault.API.Controllers;

[Route("api/auth")]
[ApiController]
[Authorize]
public sealed class AuthController(
    ILogger<AuthController> logger,
    UserManager<ApiUser> userManager,
    SignInManager<ApiUser> signInManager,
    IAccessTokenService accessTokenService,
    ICsrfTokenService csrfTokenService,
    ICaptchaService captchaService) : ControllerBase
{
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userName = loginRequest.Email;
        var password = loginRequest.Password;

        var user = await userManager.FindByNameAsync(userName);
        if (user is null)
            return Unauthorized();
        
        logger.LogInformation($"Attempting to log in user {user.Email}...");
        var result = await signInManager.CheckPasswordSignInAsync(
            user: user,
            password: password,
            lockoutOnFailure:  false);

        if (result.Succeeded)
        {
            logger.LogInformation($"User {user.Email} successfully logged in.");
            var userClaims = await userManager.GetClaimsAsync(user);

            var sessionId = Guid.NewGuid().ToString();
            var csrfToken =  await csrfTokenService.GenerateToken(sessionId);
            Response.Headers[Shared.Constants.Headers.CsrfToken] = new StringValues(csrfToken);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                SameSite = SameSiteMode.None,
                Secure = true,
                IsEssential = true,
                Expires = DateTimeOffset.Now.AddDays(accessTokenService.TokenExpirationDays),
                Path = "/"
            };
            Response.Cookies.Append(Shared.Constants.Cookies.CsrfToken, csrfToken, cookieOptions);
            
            var token = accessTokenService.CreateToken(user, sessionId, userClaims);
            var tokenResponse = new TokenResponse {Token = token};
            
            return Ok(tokenResponse);
        }

        return Unauthorized();
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var captchaValid = await captchaService.VerifyCaptcha(
            registerRequest.CaptchaCode, 
            registerRequest.CaptchaHmac);
        
        if (!captchaValid)
            return BadRequest("Invalid captcha");
        
        var username = registerRequest.Email;
        var user = new ApiUser
        {
            UserName = username,
            Email = username,
            MasterPasswordHash = registerRequest.MasterPasswordHash
        };

        var result = await userManager.CreateAsync(user, registerRequest.Password);
        if (!result.Succeeded)
            return BadRequest($"Failed to create user {username}!");
        
        logger.LogInformation($"User {user.Email} successfully registered.");
        
        return Ok();
    }
}


