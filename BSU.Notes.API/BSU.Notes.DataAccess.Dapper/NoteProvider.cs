using BSU.Notes.Data.DTOs.Note;
using BSU.Notes.DataAccess.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using Dapper;
using System.Threading.Tasks;

namespace BSU.Notes.DataAccess.Dapper
{
    public class NoteProvider : INoteProvider
    {
        private readonly string _connectionString;

        public NoteProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> CountNotes(int userId)
        {
            var query = "SELECT COUNT(*) FROM \"Note\" WHERE \"UserId\" = @UserId";

            var queryParams = new
            {
                UserId = userId
            };

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return await connection.ExecuteScalarAsync<int>(query, queryParams); ;
            }
        }

        public async Task<int> CreateNote(int userId, CreateNoteDto note)
        {
            var query = "INSERT INTO \"Note\" (\"UserId\", \"Title\", \"Content\", \"CreatedOnUTC\") " +
                "VALUES (@UserId, @Title, @Content, @CreatedOn) " +
                "RETURNING \"NoteId\"";

            var queryParams = new
            {
                UserId = userId,
                Title = note.Title,
                Content = note.Content,
                CreatedOn = DateTime.UtcNow
            };

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<int>(query, queryParams);
            }
        }

        public async Task<int> DeleteNote(int userId, int noteId)
        {
            var query = "DELETE FROM \"Note\" WHERE \"UserId\" = @UserId AND \"NoteId\" = @NoteId";

            var queryParams = new
            {
                UserId = userId,
                NoteId = noteId
            };

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return await connection.ExecuteAsync(query, queryParams);
            }
        }

        public async Task<NoteDto> GetNote(int userId, int noteId)
        {
            var query = "SELECT \"NoteId\", \"Title\", \"Content\", \"UpdatedOnUTC\", \"CreatedOnUTC\" " +
                "FROM \"Note\" " +
                "WHERE \"UserId\" = @UserId AND \"NoteId\" = @NoteId";

            var queryParams = new
            {
                UserId = userId,
                NoteId = noteId
            };

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<NoteDto>(query, queryParams);
            }
        }

        public async Task<IEnumerable<NoteDto>> GetNotes(int userId, int? page, int? size)
        {
            var paginationFilter = string.Empty;

            int? offset = null;
            if (page.HasValue && size.HasValue)
            {
                paginationFilter = "OFFSET @Offset LIMIT @Limit";
                offset = (page - 1) * size;
            }

            var query = "SELECT \"NoteId\", \"Title\", \"Content\", \"UpdatedOnUTC\", \"CreatedOnUTC\" " +
                "FROM \"Note\" " +
                "WHERE \"UserId\" = @UserId " +
                $"ORDER BY \"CreatedOnUTC\" DESC, \"UpdatedOnUTC\" DESC {paginationFilter}";

            var queryParams = new
            {
                UserId = userId,
                Limit = size,
                Offset = offset
            };

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return await connection.QueryAsync<NoteDto>(query, queryParams);
            }
        }

        public async Task<int> UpdateNote(int userId, int noteId, UpdateNoteDto note)
        {
            var query = "UPDATE \"Note\" " +
                "SET \"Title\" = @Title, \"Content\" = @Content, \"UpdatedOnUTC\" = @UpdatedOn " +
                "WHERE \"UserId\" = @UserId AND \"NoteId\" = @NoteId";

            var queryParams = new
            {
                Title = note.Title,
                Content = note.Content,
                UpdatedOn = DateTime.UtcNow,
                UserId = userId,
                NoteId = noteId
            };

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return await connection.ExecuteAsync(query, queryParams);
            }
        }
    }
}
