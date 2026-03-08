using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;

namespace SaviaVetAPI.Services
{
    public interface IClinicScheduleService
    {
        Task<List<ClinicSchedule>> GetClinicSchedulesAsync(int? franchiseId, int? roomId, int? dayOfWeek);
        Task<ClinicSchedule> GetOneClinicScheduleAsync(int id);
        Task<bool> AddClinicScheduleAsync(AddClinicScheduleDTO dto);
        Task<bool> UpdateClinicScheduleAsync(UpdateClinicScheduleDTO dto);
        Task<bool> DeleteClinicScheduleAsync(int id);
        Task<bool> IsWithinOpeningHoursAsync(int franchiseId, int? roomId, DateTime start, DateTime end);
    }
}
