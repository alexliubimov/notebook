using BSU.Notes.Data.DTOs.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BSU.Notes.DataAccess.Interfaces
{
    public interface IUserProvider
    {
        Task<IEnumerable<UserDto>> GetUsers(int? page, int? size);
        Task<UserDto> GetUser(int userId);
        Task<int> CountUsers();
        Task<int> CreateUser(CreateUserDto user);
        Task<int> UpdateUser(int userId, UpdateUserDto user);
        Task<int> DeleteUser(int userId);
    }
}
