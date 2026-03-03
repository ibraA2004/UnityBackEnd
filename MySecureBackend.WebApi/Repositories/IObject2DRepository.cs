using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public interface IObject2DRepository
    {
        Task<IEnumerable<Object2D>> GetAllAsync();
        Task<Object2D?> GetByIdAsync(string id);
        Task<IEnumerable<Object2D>> GetByEnvironmentIdAsync(string environmentId);
        Task<Object2D> CreateAsync(Object2D object2D);
        Task UpdateAsync(Object2D object2D);
        Task DeleteAsync(string id);
    }
}
