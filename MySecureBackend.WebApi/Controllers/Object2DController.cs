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
    public class Object2DController : ControllerBase
    {
        private readonly IObject2DRepository _repository;
        private readonly IAuthenticationService _authenticationService;

        public Object2DController(IObject2DRepository repository, IAuthenticationService authenticationService)
        {
            _repository = repository;
            _authenticationService = authenticationService;
        }

        [HttpGet(Name = "GetAllObjects")]
        public async Task<ActionResult<IEnumerable<Object2D>>> GetAllAsync()
        {
            var objects = await _repository.GetAllAsync();
            return Ok(objects);
        }

        [HttpGet("{id}", Name = "GetObjectById")]
        public async Task<ActionResult<Object2D>> GetByIdAsync(string id)
        {
            var obj = await _repository.GetByIdAsync(id);
            if (obj == null)
                return NotFound(new ProblemDetails { Detail = $"Object2D {id} not found" });

            return Ok(obj);
        }

        [HttpGet("environment/{environmentId}", Name = "GetObjectsByEnvironment")]
        public async Task<ActionResult<IEnumerable<Object2D>>> GetByEnvironmentAsync(string environmentId)
        {
            var objects = await _repository.GetByEnvironmentIdAsync(environmentId);
            return Ok(objects);
        }

        [HttpPost(Name = "CreateObject")]
        public async Task<ActionResult<Object2D>> CreateAsync(Object2D object2D)
        {
            object2D.Id = Guid.NewGuid().ToString();
            
            var createdObject = await _repository.CreateAsync(object2D);
            return CreatedAtRoute("GetObjectById", new { id = createdObject.Id }, createdObject);
        }

        [HttpPut("{id}", Name = "UpdateObject")]
        public async Task<ActionResult<Object2D>> UpdateAsync(string id, Object2D object2D)
        {
            var existingObject = await _repository.GetByIdAsync(id);
            if (existingObject == null)
                return NotFound(new ProblemDetails { Detail = $"Object2D {id} not found" });

            if (object2D.Id != id)
                return Conflict(new ProblemDetails { Detail = "The id in the route does not match the id in the body" });

            await _repository.UpdateAsync(object2D);
            return Ok(object2D);
        }

        [HttpDelete("{id}", Name = "DeleteObject")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            var obj = await _repository.GetByIdAsync(id);
            if (obj == null)
                return NotFound(new ProblemDetails { Detail = $"Object2D {id} not found" });

            await _repository.DeleteAsync(id);
            return Ok();
        }
    }
}
