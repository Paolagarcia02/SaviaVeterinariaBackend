using SaviaVetAPI.DTOs;

namespace SaviaVetAPI.Repositories
{
    public interface IAppointmentRepository
    {
        public Task<List<GetAppointmentDTO>> GetAppointmentsAsync();
        public Task<GetAppointmentDTO> GetOneAppointmentAsync(int id);
        public Task<bool> AddAppointmentAsync(AddAppointmentDTO addAppointmentDTO);
        public Task<bool> UpdateAppointmentAsync(UpdateAppointmentDTO updateAppointmentDTO);
        public Task<bool> DeleteAppointmentAsync(int id);
    }
}