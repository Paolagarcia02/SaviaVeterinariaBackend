using SaviaVetAPI.DTOs;

namespace SaviaVetAPI.Repositories
{
    public interface IClinicScheduleRepository
    {
        Task<List<GetClinicScheduleDTO>> GetClinicSchedulesAsync();
        Task<GetClinicScheduleDTO> GetOneClinicScheduleAsync(int id);
        Task<bool> AddClinicScheduleAsync(AddClinicScheduleDTO dto);
        Task<bool> UpdateClinicScheduleAsync(UpdateClinicScheduleDTO dto);
        Task<bool> DeleteClinicScheduleAsync(int id);
    }
}
