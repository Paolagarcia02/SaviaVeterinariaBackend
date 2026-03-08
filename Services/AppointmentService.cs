using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;
using SaviaVetAPI.Repositories;

namespace SaviaVetAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;
        private readonly IClinicScheduleService _clinicScheduleService;

        public AppointmentService(IAppointmentRepository repository, IClinicScheduleService clinicScheduleService)
        {
            _repository = repository;
            _clinicScheduleService = clinicScheduleService;
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
                models = models.FindAll(a => a.Date_time.HasValue && a.Date_time.Value.Date == date.Value.Date);
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

        public async Task<Dictionary<string, string[]>> ValidateAddAsync(AddAppointmentDTO dto)
        {
            var errors = new Dictionary<string, List<string>>();
            var status = string.IsNullOrWhiteSpace(dto.Status) ? "Pendiente" : dto.Status.Trim();
            var isPending = string.Equals(status, "Pendiente", StringComparison.OrdinalIgnoreCase);
            var hasSchedulingData = dto.Date_time != null || dto.Vet_id != null || dto.Room_id != null;

            if (dto.Franchise_id == null || dto.Franchise_id <= 0)
            {
                AddError(errors, nameof(dto.Franchise_id), "La franquicia es obligatoria.");
            }

            if (dto.Duration_minutes <= 0)
            {
                AddError(errors, nameof(dto.Duration_minutes), "La duración debe ser mayor a 0 minutos.");
            }

            // Una solicitud "Pendiente" puede crearse sin asignación.
            var requiresScheduling = !isPending || hasSchedulingData;

            if (requiresScheduling)
            {
                if (dto.Date_time == null)
                {
                    AddError(errors, nameof(dto.Date_time), "La fecha y hora son obligatorias cuando se agenda la cita.");
                }

                if (dto.Vet_id == null || dto.Vet_id <= 0)
                {
                    AddError(errors, nameof(dto.Vet_id), "El veterinario es obligatorio cuando se agenda la cita.");
                }

                if (dto.Room_id == null || dto.Room_id <= 0)
                {
                    AddError(errors, nameof(dto.Room_id), "La sala es obligatoria cuando se agenda la cita.");
                }
            }

            if (errors.Any())
            {
                return ToReadOnly(errors);
            }

            if (!requiresScheduling)
            {
                return ToReadOnly(errors);
            }

            var start = dto.Date_time!.Value;
            var end = start.AddMinutes(dto.Duration_minutes);
            var vetId = dto.Vet_id!.Value;
            var roomId = dto.Room_id!.Value;

            var withinHours = await _clinicScheduleService.IsWithinOpeningHoursAsync(dto.Franchise_id!.Value, roomId, start, end);
            if (!withinHours)
            {
                AddError(errors, nameof(dto.Date_time), "La cita está fuera del horario de apertura configurado.");
            }

            var appointments = await _repository.GetAppointmentsAsync();
            foreach (var existing in appointments.Where(a => !string.Equals(a.Status, "Cancelada", StringComparison.OrdinalIgnoreCase)))
            {
                if (existing.Date_time == null) continue;
                if (existing.Franchise_id != dto.Franchise_id!.Value) continue;

                var existingStart = existing.Date_time.Value;
                var existingEnd = existingStart.AddMinutes(existing.Duration_minutes);
                if (!Overlaps(start, end, existingStart, existingEnd)) continue;

                if (existing.Room_id == roomId)
                {
                    AddError(errors, nameof(dto.Room_id), "La sala ya está reservada en esa franja para esta franquicia.");
                }

                if (existing.Vet_id == vetId)
                {
                    AddError(errors, nameof(dto.Vet_id), "El veterinario ya tiene cita en esa franja.");
                }
            }

            return ToReadOnly(errors);
        }

        public async Task<Dictionary<string, string[]>> ValidateUpdateAsync(UpdateAppointmentDTO dto)
        {
            var errors = new Dictionary<string, List<string>>();
            var existingAppointment = await _repository.GetOneAppointmentAsync(dto.Appointment_id);
            if (existingAppointment == null)
            {
                AddError(errors, nameof(dto.Appointment_id), "La cita indicada no existe.");
                return ToReadOnly(errors);
            }

            var isStatusOnlyCancel = string.Equals(dto.Status, "Cancelada", StringComparison.OrdinalIgnoreCase)
                && dto.Date_time == null && dto.Vet_id == null && dto.Room_id == null;

            if (isStatusOnlyCancel)
            {
                return ToReadOnly(errors);
            }

            var start = dto.Date_time ?? existingAppointment.Date_time;
            var vetId = dto.Vet_id ?? existingAppointment.Vet_id;
            var roomId = dto.Room_id ?? existingAppointment.Room_id;
            var franchiseId = existingAppointment.Franchise_id;
            var nextStatus = string.IsNullOrWhiteSpace(dto.Status) ? existingAppointment.Status : dto.Status.Trim();
            var hasSchedulingData = dto.Date_time != null || dto.Vet_id != null || dto.Room_id != null;
            var requiresScheduling = !string.Equals(nextStatus, "Pendiente", StringComparison.OrdinalIgnoreCase) || hasSchedulingData;

            if (requiresScheduling && start == null)
            {
                AddError(errors, nameof(dto.Date_time), "Debe definir fecha y hora para una cita no pendiente.");
            }

            if (requiresScheduling && (vetId == null || vetId <= 0))
            {
                AddError(errors, nameof(dto.Vet_id), "Debe asignar veterinario para una cita no pendiente.");
            }

            if (requiresScheduling && (roomId == null || roomId <= 0))
            {
                AddError(errors, nameof(dto.Room_id), "Debe asignar sala para una cita no pendiente.");
            }

            if (errors.Any())
            {
                return ToReadOnly(errors);
            }

            if (!requiresScheduling)
            {
                return ToReadOnly(errors);
            }

            if (start == null || vetId == null || roomId == null)
            {
                return ToReadOnly(errors);
            }

            var end = start.Value.AddMinutes(existingAppointment.Duration_minutes);
            var withinHours = await _clinicScheduleService.IsWithinOpeningHoursAsync(franchiseId, roomId, start.Value, end);
            if (!withinHours)
            {
                AddError(errors, nameof(dto.Date_time), "La cita está fuera del horario de apertura configurado.");
            }

            var appointments = await _repository.GetAppointmentsAsync();
            foreach (var existing in appointments.Where(a =>
                         a.Appointment_id != dto.Appointment_id &&
                         !string.Equals(a.Status, "Cancelada", StringComparison.OrdinalIgnoreCase)))
            {
                if (existing.Date_time == null) continue;
                if (existing.Franchise_id != franchiseId) continue;

                var existingStart = existing.Date_time.Value;
                var existingEnd = existingStart.AddMinutes(existing.Duration_minutes);
                if (!Overlaps(start.Value, end, existingStart, existingEnd)) continue;

                if (existing.Room_id == roomId)
                {
                    AddError(errors, nameof(dto.Room_id), "La sala ya está reservada en esa franja para esta franquicia.");
                }

                if (existing.Vet_id == vetId)
                {
                    AddError(errors, nameof(dto.Vet_id), "El veterinario ya tiene cita en esa franja.");
                }
            }

            return ToReadOnly(errors);
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

        private static bool Overlaps(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
        {
            return startA < endB && startB < endA;
        }

        private static void AddError(Dictionary<string, List<string>> errors, string field, string message)
        {
            if (!errors.ContainsKey(field)) errors[field] = new List<string>();
            if (!errors[field].Contains(message)) errors[field].Add(message);
        }

        private static Dictionary<string, string[]> ToReadOnly(Dictionary<string, List<string>> errors)
        {
            return errors.ToDictionary(k => k.Key, v => v.Value.ToArray());
        }
    }
}
