using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;
using SaviaVetAPI.Repositories;

namespace SaviaVetAPI.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _repository;

        public ServiceService(IServiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Service>> GetServicesAsync(string? name)
        {
            List<GetServiceDTO> dtos = await _repository.GetServicesAsync();

            List<Service> models = new List<Service>();
            foreach (var dto in dtos)
            {
                models.Add(new Service
                {
                    Service_id = dto.Service_id,
                    Name = dto.Name,
                    Description = dto.Description,
                    Icon = dto.Icon
                });
            }

            if (!string.IsNullOrEmpty(name))
            {
                models = models.FindAll(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            return models;
        }

        public async Task<Service> GetOneServiceAsync(int id)
        {
            GetServiceDTO dto = await _repository.GetOneServiceAsync(id);
            if (dto == null) return null;

            return new Service
            {
                Service_id = dto.Service_id,
                Name = dto.Name,
                Description = dto.Description,
                Icon = dto.Icon
            };
        }

        public async Task<bool> AddServiceAsync(AddServiceDTO dto)
        {
            return await _repository.AddServiceAsync(dto);
        }

        public async Task<bool> UpdateServiceAsync(UpdateServiceDTO dto)
        {
            return await _repository.UpdateServiceAsync(dto);
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            return await _repository.DeleteServiceAsync(id);
        }
    }
}