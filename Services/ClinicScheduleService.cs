using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;
using SaviaVetAPI.Repositories;

namespace SaviaVetAPI.Services
{
    public class ClinicScheduleService : IClinicScheduleService
    {
        private readonly IClinicScheduleRepository _repository;

        public ClinicScheduleService(IClinicScheduleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ClinicSchedule>> GetClinicSchedulesAsync(int? franchiseId, int? roomId, int? dayOfWeek)
        {
            var dtos = await _repository.GetClinicSchedulesAsync();
            var models = dtos.Select(dto => new ClinicSchedule
            {
                Schedule_id = dto.Schedule_id,
                Franchise_id = dto.Franchise_id,
                Room_id = dto.Room_id,
                Day_of_week = dto.Day_of_week,
                Open_time = dto.Open_time,
                Close_time = dto.Close_time,
                Is_open = dto.Is_open
            }).ToList();

            if (franchiseId != null) models = models.Where(s => s.Franchise_id == franchiseId).ToList();
            if (roomId != null) models = models.Where(s => s.Room_id == roomId).ToList();
            if (dayOfWeek != null) models = models.Where(s => s.Day_of_week == dayOfWeek).ToList();

            return models;
        }

        public async Task<ClinicSchedule> GetOneClinicScheduleAsync(int id)
        {
            var dto = await _repository.GetOneClinicScheduleAsync(id);
            if (dto == null) return null;

            return new ClinicSchedule
            {
                Schedule_id = dto.Schedule_id,
                Franchise_id = dto.Franchise_id,
                Room_id = dto.Room_id,
                Day_of_week = dto.Day_of_week,
                Open_time = dto.Open_time,
                Close_time = dto.Close_time,
                Is_open = dto.Is_open
            };
        }

        public Task<bool> AddClinicScheduleAsync(AddClinicScheduleDTO dto) => _repository.AddClinicScheduleAsync(dto);
        public Task<bool> UpdateClinicScheduleAsync(UpdateClinicScheduleDTO dto) => _repository.UpdateClinicScheduleAsync(dto);
        public Task<bool> DeleteClinicScheduleAsync(int id) => _repository.DeleteClinicScheduleAsync(id);

        public async Task<bool> IsWithinOpeningHoursAsync(int franchiseId, int? roomId, DateTime start, DateTime end)
        {
            if (end <= start || start.Date != end.Date) return false;

            var day = (int)start.DayOfWeek;
            var schedules = await GetClinicSchedulesAsync(franchiseId, null, day);

            var roomSchedule = roomId != null ? schedules.FirstOrDefault(s => s.Room_id == roomId) : null;
            var franchiseSchedule = schedules.FirstOrDefault(s => s.Room_id == null);
            var selected = roomSchedule ?? franchiseSchedule;

            if (selected == null || !selected.Is_open) return false;

            var startTime = start.TimeOfDay;
            var endTime = end.TimeOfDay;
            return startTime >= selected.Open_time && endTime <= selected.Close_time;
        }
    }
}
