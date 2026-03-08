using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;

namespace SaviaVetAPI.Services
{
    public interface IAppointmentService
    {
        // Filtros: por fecha o por veterinario
        Task<List<Appointment>> GetAppointmentsAsync(DateTime? date, int? vetId);
        Task<Appointment> GetOneAppointmentAsync(int id);
        Task<Dictionary<string, string[]>> ValidateAddAsync(AddAppointmentDTO dto);
        Task<Dictionary<string, string[]>> ValidateUpdateAsync(UpdateAppointmentDTO dto);
        
        Task<bool> AddAppointmentAsync(AddAppointmentDTO dto);
        Task<bool> UpdateAppointmentAsync(UpdateAppointmentDTO dto);
        Task<bool> DeleteAppointmentAsync(int id);
    }
}
