using BSU.Notes.Data.DTOs.User;
using BSU.Notes.DataAccess.Interfaces;
using System.Collections.Generic;
using Dapper;
using Npgsql;
using System.Threading.Tasks;
using System;

namespace BSU.Notes.DataAccess.Dapper
{
    public class UserProvider : IUserProvider
    {
        private readonly string _connectionString;

        public UserProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> CountUsers()
        {
            var query = "SELECT COUNT(*) FROM \"User\"";

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return await connection.ExecuteScalarAsync<int>(query);
            }
        }

        public async Task<int> CreateUser(CreateUserDto user)
        {
            var query = "INSERT INTO \"User\" (\"UserName\", \"Email\") VALUES (@UserName, @Email) RETURNING \"UserId\"";

            var queryParams = new
            {
                UserName = user.UserName,
                Email = user.Email
            };

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<int>(query, queryParams);
            }
        }

        public async Task<int> DeleteUser(int userId)
        {
            var query = "DELETE FROM \"User\" WHERE \"UserId\" = @UserId";
            
            var queryParams = new
            {
                UserId = userId
            };

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return await connection.ExecuteAsync(query, queryParams);
            }
        }

        public async Task<UserDto> GetUser(int userId)
        {
            var query = "SELECT * FROM \"User\" WHERE \"UserId\" = @UserId";

            var queryParams = new
            {
                UserId = userId
            };

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<UserDto>(query, queryParams);
            }
        }

        public async Task<IEnumerable<UserDto>> GetUsers(int? page, int? size)
        {
            var paginationFilter = string.Empty;

            int? offset = null;
            if (page.HasValue && size.HasValue)
            {
                paginationFilter = "OFFSET @Offset LIMIT @Limit";
                offset = (page - 1) * size;
            }

            var query = $"SELECT * FROM \"User\" ORDER BY \"UserId\" ASC {paginationFilter}";

            var queryParams = new
            {
                Limit = size,
                Offset = offset
            };

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return await connection.QueryAsync<UserDto>(query, queryParams);
            }
        }

        public async Task<int> UpdateUser(int userId, UpdateUserDto user)
        {
            var query = "UPDATE \"User\" SET \"UserName\" = @UserName, \"Email\" = @Email WHERE \"UserId\" = @UserId";

            var queryParams = new
            {
                UserName = user.UserName,
                Email = user.Email,
                UserId = userId
            };

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return await connection.ExecuteAsync(query, queryParams);
            }
        }
    }
}
