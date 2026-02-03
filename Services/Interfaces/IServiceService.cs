using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;

namespace SaviaVetAPI.Services
{
    public interface IServiceService
    {
        // Filtros: por nombre
        Task<List<Service>> GetServicesAsync(string? name);
        Task<Service> GetOneServiceAsync(int id);
        
        Task<bool> AddServiceAsync(AddServiceDTO dto);
        Task<bool> UpdateServiceAsync(UpdateServiceDTO dto);
        Task<bool> DeleteServiceAsync(int id);
    }
}