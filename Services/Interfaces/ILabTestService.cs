using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;

namespace SaviaVetAPI.Services
{
    public interface ILabTestService
    {
        // Filtros: por cita o estado
        Task<List<LabTest>> GetLabTestsAsync(int? appointmentId, string? status);
        Task<LabTest> GetOneLabTestAsync(int id);
        
        Task<bool> AddLabTestAsync(AddLabTestDTO dto);
        Task<bool> UpdateLabTestAsync(UpdateLabTestDTO dto);
        Task<bool> DeleteLabTestAsync(int id);
    }
}