using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;

namespace SaviaVetAPI.Services
{
    public interface IUserService
    {
        // Filtros: por rol o nombre
        Task<List<User>> GetUsersAsync(string? role, string? name);
        Task<User> GetOneUserAsync(int id);
        Task<bool> AddUserAsync(AddUserDTO dto);
        Task<bool> UpdateUserAsync(UpdateUserDTO dto);
        Task<bool> DeleteUserAsync(int id);
    }
}