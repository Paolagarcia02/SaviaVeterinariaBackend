using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;

namespace SaviaVetAPI.Services
{
    public interface IFranchiseService
    {
        // Filtros: por nombre
        Task<List<Franchise>> GetFranchisesAsync(string? name);
        Task<Franchise> GetOneFranchiseAsync(int id);
        
        Task<bool> AddFranchiseAsync(AddFranchiseDTO dto);
        Task<bool> UpdateFranchiseAsync(UpdateFranchiseDTO dto);
        Task<bool> DeleteFranchiseAsync(int id);
    }
}