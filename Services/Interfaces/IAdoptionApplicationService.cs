using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;

namespace SaviaVetAPI.Services
{
    public interface IAdoptionApplicationService
    {
        // Filtros: por usuario o por estado
        Task<List<AdoptionApplication>> GetAdoptionApplicationsAsync(int? userId, string? status);
        Task<AdoptionApplication> GetOneAdoptionApplicationAsync(int id);
        
        Task<bool> AddAdoptionApplicationAsync(AddAdoptionApplicationDTO dto);
        Task<bool> UpdateAdoptionApplicationAsync(UpdateAdoptionApplicationDTO dto);
        Task<bool> DeleteAdoptionApplicationAsync(int id);
    }
}