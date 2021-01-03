using BSU.Notes.Data;
using BSU.Notes.Data.DTOs.User;
using BSU.Notes.DataAccess.Interfaces;
using BSU.Notes.Models.Exceptions;
using BSU.Notes.Services.Interfaces;
using System.Threading.Tasks;

namespace BSU.Notes.Services
{
    public class UserService : IUserService
    {
        private readonly IUserProvider _userProvider;

        public UserService(IUserProvider userProvider)
        {
            _userProvider = userProvider;
        }

        public async Task<int> CreateUser(CreateUserDto user)
        {
            return await _userProvider.CreateUser(user);
        }

        public async Task DeleteUser(int userId)
        {
            var result = await _userProvider.DeleteUser(userId);

            if (result == 0)
            {
                throw new NotFoundException();
            }
        }

        public async Task<UserDto> GetUser(int userId)
        {
            var user = await _userProvider.GetUser(userId);

            if (user == null)
            {
                throw new NotFoundException();
            }

            return user;
        }

        public async Task<ItemsResponseModel<UserDto>> GetUsers(int? page, int? size)
        {
            var usersTask = _userProvider.GetUsers(page, size);

            if (page.HasValue && size.HasValue)
            {
                
                var countTask = _userProvider.CountUsers();

                return new ItemsResponseModel<UserDto>
                {
                    Items = await usersTask,
                    PaginationInfo = new PaginationModel
                    {
                        Page = page.Value,
                        Size = size.Value,
                        TotalRecords = await countTask
                    }
                };
            }

            return new ItemsResponseModel<UserDto>
            {
                Items = await usersTask,
                PaginationInfo = null
            };
        }

        public async Task UpdateUser(int userId, UpdateUserDto user)
        {
            var result = await _userProvider.UpdateUser(userId, user);

            if (result == 0)
            {
                throw new NotFoundException();
            }
        }
    }
}
