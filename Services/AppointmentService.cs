using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;
using SaviaVetAPI.Repositories;

namespace SaviaVetAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;

        public AppointmentService(IAppointmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Appointment>> GetAppointmentsAsync(DateTime? date, int? vetId)
        {
            List<GetAppointmentDTO> dtos = await _repository.GetAppointmentsAsync();
            
            List<Appointment> models = new List<Appointment>();
            foreach (var dto in dtos)
            {
                models.Add(new Appointment
                {
                    Appointment_id = dto.Appointment_id,
                    Date_time = dto.Date_time,
                    Duration_minutes = dto.Duration_minutes,
                    Reason = dto.Reason,
                    Status = dto.Status,
                    Notes = dto.Notes,
                    Pet_id = dto.Pet_id,
                    Vet_id = dto.Vet_id,
                    Franchise_id = dto.Franchise_id,
                    Room_id = dto.Room_id
                });
            }

            if (date != null)
            {
                // Filtramos por el mismo día
                models = models.FindAll(a => a.Date_time.Date == date.Value.Date);
            }
            if (vetId != null)
            {
                models = models.FindAll(a => a.Vet_id == vetId);
            }

            return models;
        }

        public async Task<Appointment> GetOneAppointmentAsync(int id)
        {
            GetAppointmentDTO dto = await _repository.GetOneAppointmentAsync(id);
            if (dto == null) return null;

            return new Appointment
            {
                Appointment_id = dto.Appointment_id,
                Date_time = dto.Date_time,
                Duration_minutes = dto.Duration_minutes,
                Reason = dto.Reason,
                Status = dto.Status,
                Notes = dto.Notes,
                Pet_id = dto.Pet_id,
                Vet_id = dto.Vet_id,
                Franchise_id = dto.Franchise_id,
                Room_id = dto.Room_id
            };
        }

        public async Task<bool> AddAppointmentAsync(AddAppointmentDTO dto)
        {
            return await _repository.AddAppointmentAsync(dto);
        }

        public async Task<bool> UpdateAppointmentAsync(UpdateAppointmentDTO dto)
        {
            return await _repository.UpdateAppointmentAsync(dto);
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            return await _repository.DeleteAppointmentAsync(id);
        }
    }
}