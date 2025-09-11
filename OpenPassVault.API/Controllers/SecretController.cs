using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenPassVault.API.Data.DataContext;
using OpenPassVault.API.Data.Entity;
using OpenPassVault.API.Data.Interfaces;

namespace OpenPassVault.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecretController(ISecretRepository secretRepository, UserManager<ApiUser> userManager) : ControllerBase
    {
        // GET: api/Secret
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Secret>>> GetSecret()
        {
            return Ok();
        }

        // GET: api/Secret/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Secret>> GetSecret(string id)
        {
            return Ok();
        }

        // PUT: api/Secret/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSecret(string id, Secret secret)
        {
            return NoContent();
        }

        // POST: api/Secret
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Secret>> PostSecret(Secret secret)
        {
            return CreatedAtAction("GetSecret", new { id = secret.Id }, secret);
        }

        // DELETE: api/Secret/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSecret(string id)
        {
            return Ok();
        }
    }
}
