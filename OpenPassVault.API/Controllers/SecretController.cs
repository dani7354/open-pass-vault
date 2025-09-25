using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenPassVault.API.Data.Entity;
using OpenPassVault.API.Data.Exceptions;
using OpenPassVault.API.Services.Interfaces;
using OpenPassVault.Shared.DTO;
using Secret = OpenPassVault.API.Data.Entity.Secret;

namespace OpenPassVault.API.Controllers;

[Route("api/secrets")]
[ApiController]
[Authorize]
public sealed class SecretController(ISecretService secretService, UserManager<ApiUser> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Secrets()
    {
        var secretsForUser = await userManager.GetUserAsync(HttpContext.User);
        if (secretsForUser == null)
            return Unauthorized();
        
        var secrets = await secretService.ListAsync(secretsForUser.Id);
        return Ok(secrets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Secret>> Secret(string id)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user == null)
            return Unauthorized();
        
        var secretDetails = await secretService.GetAsync(id, user.Id);
        if (secretDetails == null)
            return NotFound();
        
        return Ok(secretDetails);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, SecretUpdateRequest updateRequest)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user == null)
            return Unauthorized();
        
        if (id != updateRequest.Id)
            return BadRequest("Id in URL does not match Id in body");
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        try
        {
            await secretService.UpdateAsync(updateRequest, id);
        }
        catch (NotFoundException)
        {
            return BadRequest();
        }

        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult> Create(SecretCreateRequest createRequest)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user == null)
            return Unauthorized();
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var secretId = await secretService.CreateAsync(createRequest, user.Id);
        var secret = await secretService.GetAsync(secretId, user.Id);
        
        return CreatedAtAction("Secrets", new { id = secretId }, secret);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user == null)
            return Unauthorized();

        try
        {
            await secretService.DeleteAsync(id, user.Id);
        }
        catch(NotFoundException)
        {
            return NotFound();
        }
        
        return Ok();
    }
}
