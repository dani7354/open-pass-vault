using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using OpenPassVault.API.Data.Entity;
using OpenPassVault.API.Services.Interfaces;
using OpenPassVault.Shared.DTO;
using OpenPassVault.Shared.Services.Interfaces;
using OpenPassVault.Shared.Validation.Attributes;

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
    
    [HttpGet("user-info")]
    public async Task<IActionResult> UserInfo()
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user == null)
            return Unauthorized();

        var userInfoResponse = new UserInfoResponse
        {
            Id = user.Id,
            Email = user.Email!
        };
        
        return Ok(userInfoResponse);
    }
    
    [HttpPut("user-info")]
    public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserRequest updateUserRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user == null)
            return Unauthorized();
        
        if (user.Id != updateUserRequest.Id)
            return BadRequest("User ID mismatch!");
        
        var captchaValid = await captchaService.VerifyCaptcha(
            updateUserRequest.CaptchaCode, 
            updateUserRequest.CaptchaHmac);
        
        if (!captchaValid)
            return BadRequest("CAPTCHA invalid!");
        
        var passwordValid = await userManager.CheckPasswordAsync(user, updateUserRequest.CurrentPassword);
        if (!passwordValid)
            return BadRequest("Current password is incorrect!");
        
        IdentityResult result;
        if (!string.IsNullOrEmpty(updateUserRequest.NewPassword))
        {
            result = await userManager.ChangePasswordAsync(user, updateUserRequest.CurrentPassword, updateUserRequest.NewPassword);
            if (!result.Succeeded)
                return BadRequest("Failed to update password!");
        }

        if (!string.IsNullOrEmpty(updateUserRequest.MasterPasswordHash))
        {
            user.MasterPasswordHash = updateUserRequest.MasterPasswordHash;
            result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest("Failed to update user info!");
        }
        
        logger.LogInformation($"User {user.Email} info successfully updated.");
        
        return Ok();
    }
    
    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(
        [FromQuery, ValidCaptchaCodeFormat] string captchaCode,
        [FromQuery] string captchaHmac)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user == null)
            return Unauthorized();
        
        var captchaValid = await captchaService.VerifyCaptcha(captchaCode, captchaHmac);
        if (!captchaValid)
            return BadRequest("CAPTCHA invalid!");
        
        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest("Failed to delete user account!");
        
        logger.LogInformation($"User {user.Email} account successfully deleted.");
        
        return Ok();
    }
}


