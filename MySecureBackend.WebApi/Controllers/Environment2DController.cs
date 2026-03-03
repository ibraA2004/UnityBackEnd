using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySecureBackend.WebApi.Models;
using MySecureBackend.WebApi.Repositories;
using MySecureBackend.WebApi.Services;

namespace MySecureBackend.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class Environment2DController : ControllerBase
    {
        private readonly IEnvironment2DRepository _repository;
        private readonly IAuthenticationService _authenticationService;

        public Environment2DController(IEnvironment2DRepository repository, IAuthenticationService authenticationService)
        {
            _repository = repository;
            _authenticationService = authenticationService;
        }

        [HttpGet(Name = "GetAllEnvironments")]
        public async Task<ActionResult<IEnumerable<Environment2D>>> GetAllAsync()
        {
            var environments = await _repository.GetAllAsync();
            return Ok(environments);
        }

        [HttpGet("{id}", Name = "GetEnvironmentById")]
        public async Task<ActionResult<Environment2D>> GetByIdAsync(string id)
        {
            var environment = await _repository.GetByIdAsync(id);
            if (environment == null)
                return NotFound(new ProblemDetails { Detail = $"Environment2D {id} not found" });

            return Ok(environment);
        }

        [HttpGet("user/{ownerUserId}", Name = "GetEnvironmentsByOwner")]
        public async Task<ActionResult<IEnumerable<Environment2D>>> GetByOwnerAsync(string ownerUserId)
        {
            var environments = await _repository.GetByOwnerUserIdAsync(ownerUserId);
            return Ok(environments);
        }

        [HttpPost(Name = "CreateEnvironment")]
        public async Task<ActionResult<Environment2D>> CreateAsync(Environment2D environment)
        {
            environment.Id = Guid.NewGuid().ToString();
            
            var createdEnvironment = await _repository.CreateAsync(environment);
            return CreatedAtRoute("GetEnvironmentById", new { id = createdEnvironment.Id }, createdEnvironment);
        }

        [HttpPut("{id}", Name = "UpdateEnvironment")]
        public async Task<ActionResult<Environment2D>> UpdateAsync(string id, Environment2D environment)
        {
            var existingEnvironment = await _repository.GetByIdAsync(id);
            if (existingEnvironment == null)
                return NotFound(new ProblemDetails { Detail = $"Environment2D {id} not found" });

            if (environment.Id != id)
                return Conflict(new ProblemDetails { Detail = "The id in the route does not match the id in the body" });

            await _repository.UpdateAsync(environment);
            return Ok(environment);
        }

        [HttpDelete("{id}", Name = "DeleteEnvironment")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            var environment = await _repository.GetByIdAsync(id);
            if (environment == null)
                return NotFound(new ProblemDetails { Detail = $"Environment2D {id} not found" });

            await _repository.DeleteAsync(id);
            return Ok();
        }
    }
}
