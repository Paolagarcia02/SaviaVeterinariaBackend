using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;
using SaviaVetAPI.Repositories;

namespace SaviaVetAPI.Services
{
    public class LabTestService : ILabTestService
    {
        private readonly ILabTestRepository _repository;

        public LabTestService(ILabTestRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<LabTest>> GetLabTestsAsync(int? appointmentId, string? status)
        {
            List<GetLabTestDTO> dtos = await _repository.GetLabTestsAsync();

            List<LabTest> models = new List<LabTest>();
            foreach (var dto in dtos)
            {
                models.Add(new LabTest
                {
                    Test_id = dto.Test_id,
                    Appointment_id = dto.Appointment_id,
                    Test_type = dto.Test_type,
                    Result_data = dto.Result_data,
                    Comments = dto.Comments,
                    Status = dto.Status,
                    Requested_at = dto.Requested_at,
                    Completed_at = dto.Completed_at
                });
            }

            if (appointmentId != null)
            {
                models = models.FindAll(t => t.Appointment_id == appointmentId);
            }
            if (!string.IsNullOrEmpty(status))
            {
                models = models.FindAll(t => t.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            return models;
        }

        public async Task<LabTest> GetOneLabTestAsync(int id)
        {
            GetLabTestDTO dto = await _repository.GetOneLabTestAsync(id);
            if (dto == null) return null;

            return new LabTest
            {
                Test_id = dto.Test_id,
                Appointment_id = dto.Appointment_id,
                Test_type = dto.Test_type,
                Result_data = dto.Result_data,
                Comments = dto.Comments,
                Status = dto.Status,
                Requested_at = dto.Requested_at,
                Completed_at = dto.Completed_at
            };
        }

        public async Task<bool> AddLabTestAsync(AddLabTestDTO dto)
        {
            return await _repository.AddLabTestAsync(dto);
        }

        public async Task<bool> UpdateLabTestAsync(UpdateLabTestDTO dto)
        {
            return await _repository.UpdateLabTestAsync(dto);
        }

        public async Task<bool> DeleteLabTestAsync(int id)
        {
            return await _repository.DeleteLabTestAsync(id);
        }
    }
}