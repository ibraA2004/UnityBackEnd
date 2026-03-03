using Dapper;
using Microsoft.Data.SqlClient;
using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public class SqlObject2DRepository : IObject2DRepository
    {
        private readonly string sqlConnectionString;

        public SqlObject2DRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<IEnumerable<Object2D>> GetAllAsync()
        {
            using var connection = new SqlConnection(sqlConnectionString);
            return await connection.QueryAsync<Object2D>(
                "SELECT Id, EnvironmentId, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer FROM [Object2D]"
            );
        }

        public async Task<Object2D?> GetByIdAsync(string id)
        {
            using var connection = new SqlConnection(sqlConnectionString);
            return await connection.QuerySingleOrDefaultAsync<Object2D>(
                "SELECT Id, EnvironmentId, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer FROM [Object2D] WHERE Id = @Id",
                new { Id = id }
            );
        }

        public async Task<IEnumerable<Object2D>> GetByEnvironmentIdAsync(string environmentId)
        {
            using var connection = new SqlConnection(sqlConnectionString);
            return await connection.QueryAsync<Object2D>(
                "SELECT Id, EnvironmentId, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer FROM [Object2D] WHERE EnvironmentId = @EnvironmentId",
                new { EnvironmentId = environmentId }
            );
        }

        public async Task<Object2D> CreateAsync(Object2D object2D)
        {
            using var connection = new SqlConnection(sqlConnectionString);
            await connection.ExecuteAsync(
                "INSERT INTO [Object2D] (Id, EnvironmentId, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer) " +
                "VALUES (@Id, @EnvironmentId, @PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer)",
                object2D
            );
            return object2D;
        }

        public async Task UpdateAsync(Object2D object2D)
        {
            using var connection = new SqlConnection(sqlConnectionString);
            await connection.ExecuteAsync(
                "UPDATE [Object2D] SET EnvironmentId = @EnvironmentId, PrefabId = @PrefabId, " +
                "PositionX = @PositionX, PositionY = @PositionY, ScaleX = @ScaleX, ScaleY = @ScaleY, " +
                "RotationZ = @RotationZ, SortingLayer = @SortingLayer WHERE Id = @Id",
                object2D
            );
        }

        public async Task DeleteAsync(string id)
        {
            using var connection = new SqlConnection(sqlConnectionString);
            await connection.ExecuteAsync(
                "DELETE FROM [Object2D] WHERE Id = @Id",
                new { Id = id }
            );
        }
    }
}
