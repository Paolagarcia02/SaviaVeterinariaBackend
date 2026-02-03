using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;
using SaviaVetAPI.Repositories;

namespace SaviaVetAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<User>> GetUsersAsync(string? role, string? name)
        {
            List<GetUserDTO> dtos = await _repository.GetUsersAsync();

            List<User> models = new List<User>();
            foreach (var dto in dtos)
            {
                models.Add(new User
                {
                    User_id = dto.User_id,
                    Full_name = dto.Full_name,
                    Email = dto.Email,
                    Role = dto.Role,
                    Franchise_id = dto.Franchise_id,
                });
            }

            if (!string.IsNullOrEmpty(role))
            {
                models = models.FindAll(u => u.Role.Equals(role, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(name))
            {
                models = models.FindAll(u => u.Full_name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            return models;
        }

        public async Task<User> GetOneUserAsync(int id)
        {
            GetUserDTO dto = await _repository.GetOneUserAsync(id);
            if (dto == null) return null;

            return new User
            {
                User_id = dto.User_id,
                Full_name = dto.Full_name,
                Email = dto.Email,
                Role = dto.Role,
                Franchise_id = dto.Franchise_id
            };
        }


        public async Task<bool> AddUserAsync(AddUserDTO dto)
        {
            return await _repository.AddUserAsync(dto);
        }

        public async Task<bool> UpdateUserAsync(UpdateUserDTO dto)
        {
            return await _repository.UpdateUserAsync(dto);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _repository.DeleteUserAsync(id);
        }
    }
}