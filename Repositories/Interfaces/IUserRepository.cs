using SaviaVetAPI.DTOs;

namespace SaviaVetAPI.Repositories
{
    public interface IUserRepository
    {
        public Task<List<GetUserDTO>> GetUsersAsync();
        public Task<GetUserDTO> GetOneUserAsync(int id);
        public Task<bool> AddUserAsync(AddUserDTO addUserDTO);
        public Task<bool> UpdateUserAsync(UpdateUserDTO updateUserDTO);
        public Task<bool> DeleteUserAsync(int id);
        public Task<UserDtoOut> GetUserByCredentialsAsync(LoginDtoIn loginDto);

        public Task<UserDtoOut> RegisterUserAsync(UserDtoIn userDto);
    }
}
