using BSU.Notes.Data;
using BSU.Notes.Data.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BSU.Notes.Services.Interfaces
{
    public interface IUserService
    {
        Task<ItemsResponseModel<UserDto>> GetUsers(int? page, int? size);
        Task<UserDto> GetUser(int userId);
        Task<int> CreateUser(CreateUserDto user);
        Task UpdateUser(int userId, UpdateUserDto user);
        Task DeleteUser(int userId);
    }
}
