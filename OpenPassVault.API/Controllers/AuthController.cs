using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenPassVault.API.Data.Entity;
using OpenPassVault.API.Services.Interfaces;
using OpenPassVault.Shared.DTO;

namespace OpenPassVault.API.Controllers;

[Route("api/auth")]
[ApiController]
public sealed class AuthController(
    ILogger<AuthController> logger,
    UserManager<ApiUser> userManager,
    SignInManager<ApiUser> signInManager,
    ITokenService tokenService) : ControllerBase
{
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userName = loginRequest.Email;
        var password = loginRequest.Password;
        var result = await signInManager.PasswordSignInAsync(
            userName: userName,
            password: password,
            isPersistent: false,
            lockoutOnFailure:  false);

        if (result.Succeeded)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user is null)
                return BadRequest("An error occurred while logging in!");

            var userClaims = await userManager.GetClaimsAsync(user);

            var token = tokenService.CreateToken(user, userClaims);
            var tokenResponse = new TokenResponse(token);

            return Ok(tokenResponse);
        }

        return Unauthorized();
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var username = registerRequest.Email;
        var user = new ApiUser()
        {
            UserName = username,
            Email = username,
            MasterPasswordHash = registerRequest.MasterPasswordHash
        };

        var result = await userManager.CreateAsync(user, registerRequest.Password);
        if (!result.Succeeded)
            return BadRequest($"Failed to create user {username}!");
        
        return Ok();
    }
}


