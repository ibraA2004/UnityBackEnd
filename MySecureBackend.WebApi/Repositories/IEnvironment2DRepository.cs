using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public interface IEnvironment2DRepository
    {
        Task<IEnumerable<Environment2D>> GetAllAsync();
        Task<Environment2D?> GetByIdAsync(string id);
        Task<IEnumerable<Environment2D>> GetByOwnerUserIdAsync(string ownerUserId);
        Task<Environment2D> CreateAsync(Environment2D environment);
        Task UpdateAsync(Environment2D environment);
        Task DeleteAsync(string id);
    }
}
