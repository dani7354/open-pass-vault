using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenPassVault.API.Data.Entity;
using OpenPassVault.API.Services.Interfaces;
using OpenPassVault.Shared.DTO;

namespace OpenPassVault.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthController(
    ILogger<AuthController> logger,
    UserManager<ApiUser> userManager,
    SignInManager<ApiUser> signInManager,
    ITokenService tokenService,
    IConfiguration configuration) : ControllerBase
{
    [HttpGet("Get")] // Delete
    public IActionResult Get()
    {
        return Ok("Hello World");
    }

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

            return Ok();
        }

        return Unauthorized();
    }
}


