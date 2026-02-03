using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;
using SaviaVetAPI.Repositories;

namespace SaviaVetAPI.Services
{
    public class AdoptionApplicationService : IAdoptionApplicationService
    {
        private readonly IAdoptionApplicationRepository _repository;

        public AdoptionApplicationService(IAdoptionApplicationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<AdoptionApplication>> GetAdoptionApplicationsAsync(int? userId, string? status)
        {
            List<GetAdoptionApplicationDTO> dtos = await _repository.GetAdoptionApplicationsAsync();

            List<AdoptionApplication> models = new List<AdoptionApplication>();
            foreach (var dto in dtos)
            {
                models.Add(new AdoptionApplication
                {
                    Application_id = dto.Application_id,
                    User_id = dto.User_id,
                    Pet_id = dto.Pet_id,
                    Message = dto.Message,
                    Status = dto.Status,
                    Application_date = dto.Application_date
                });
            }
            if (userId != null)
            {
                models = models.FindAll(a => a.User_id == userId);
            }
            if (!string.IsNullOrEmpty(status))
            {
                // StringComparison.OrdinalIgnoreCase permite buscar "pendiente" aunque esté guardado "Pendiente"
                models = models.FindAll(a => a.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            return models;
        }

        public async Task<AdoptionApplication> GetOneAdoptionApplicationAsync(int id)
        {
            GetAdoptionApplicationDTO dto = await _repository.GetOneAdoptionApplicationAsync(id);
            if (dto == null) return null;

            return new AdoptionApplication
            {
                Application_id = dto.Application_id,
                User_id = dto.User_id,
                Pet_id = dto.Pet_id,
                Message = dto.Message,
                Status = dto.Status,
                Application_date = dto.Application_date
            };
        }

        public async Task<bool> AddAdoptionApplicationAsync(AddAdoptionApplicationDTO dto)
        {
            return await _repository.AddAdoptionApplicationAsync(dto);
        }

        public async Task<bool> UpdateAdoptionApplicationAsync(UpdateAdoptionApplicationDTO dto)
        {
            return await _repository.UpdateAdoptionApplicationAsync(dto);
        }

        public async Task<bool> DeleteAdoptionApplicationAsync(int id)
        {
            return await _repository.DeleteAdoptionApplicationAsync(id);
        }
    }
}