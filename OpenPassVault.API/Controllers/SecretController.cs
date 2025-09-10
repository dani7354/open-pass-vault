using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenPassVault.API.Data.Interfaces;
using OpenPassVault.Shared.DTO;

namespace OpenPassVault.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecretController(ISecretRepository secretRepository) : ControllerBase
    {
        // GET: api/<SecretController>
        [HttpGet]
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<SecretController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var secret = await secretRepository.GetById(id);
            throw new NotImplementedException();
        }

        // POST api/<SecretController>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Secret createSecretRequest)
        {
            //await secretRepository.Add()
            throw new NotImplementedException();
        }

        // PUT api/<SecretController>/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, [FromBody] string value)
        //{
        //}

        // DELETE api/<SecretController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await secretRepository.Delete(id);
            return Ok();
        }
    }
}
