using Dapper;
using Microsoft.Data.SqlClient;
using MySecureBackend.WebApi.Models;

namespace MySecureBackend.WebApi.Repositories
{
    public class SqlEnvironment2DRepository : IEnvironment2DRepository
    {
        private readonly string sqlConnectionString;

        public SqlEnvironment2DRepository(string sqlConnectionString)
        {
            this.sqlConnectionString = sqlConnectionString;
        }

        public async Task<IEnumerable<Environment2D>> GetAllAsync()
        {
            using var connection = new SqlConnection(sqlConnectionString);
            return await connection.QueryAsync<Environment2D>(
                "SELECT Id, Name, OwnerUserId, MaxLength, MaxHeight FROM [Environment2D]"
            );
        }

        public async Task<Environment2D?> GetByIdAsync(string id)
        {
            using var connection = new SqlConnection(sqlConnectionString);
            return await connection.QuerySingleOrDefaultAsync<Environment2D>(
                "SELECT Id, Name, OwnerUserId, MaxLength, MaxHeight FROM [Environment2D] WHERE Id = @Id",
                new { Id = id }
            );
        }

        public async Task<IEnumerable<Environment2D>> GetByOwnerUserIdAsync(string ownerUserId)
        {
            using var connection = new SqlConnection(sqlConnectionString);
            return await connection.QueryAsync<Environment2D>(
                "SELECT Id, Name, OwnerUserId, MaxLength, MaxHeight FROM [Environment2D] WHERE OwnerUserId = @OwnerUserId",
                new { OwnerUserId = ownerUserId }
            );
        }

        public async Task<Environment2D> CreateAsync(Environment2D environment)
        {
            // Zorg dat Id gezet is
            if (string.IsNullOrEmpty(environment.Id))
            {
                environment.Id = Guid.NewGuid().ToString();
            }

            using var connection = new SqlConnection(sqlConnectionString);
            await connection.ExecuteAsync(
                "INSERT INTO [Environment2D] (Id, Name, OwnerUserId, MaxLength, MaxHeight) VALUES (@Id, @Name, @OwnerUserId, @MaxLength, @MaxHeight)",
                environment
            );

            return environment;
        }

        public async Task UpdateAsync(Environment2D environment)
        {
            using var connection = new SqlConnection(sqlConnectionString);
            await connection.ExecuteAsync(
                "UPDATE [Environment2D] SET Name = @Name, OwnerUserId = @OwnerUserId, MaxLength = @MaxLength, MaxHeight = @MaxHeight WHERE Id = @Id",
                environment
            );
        }

        public async Task DeleteAsync(string id)
        {
            using var connection = new SqlConnection(sqlConnectionString);
            await connection.ExecuteAsync(
                "DELETE FROM [Environment2D] WHERE Id = @Id",
                new { Id = id }
            );
        }
    }
}
