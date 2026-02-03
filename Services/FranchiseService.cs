using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;
using SaviaVetAPI.Repositories;

namespace SaviaVetAPI.Services
{
    public class FranchiseService : IFranchiseService
    {
        private readonly IFranchiseRepository _repository;

        public FranchiseService(IFranchiseRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Franchise>> GetFranchisesAsync(string? name)
        {
            List<GetFranchiseDTO> dtos = await _repository.GetFranchisesAsync();

            List<Franchise> models = new List<Franchise>();
            foreach (var dto in dtos)
            {
                models.Add(new Franchise
                {
                    Franchise_id = dto.Franchise_id,
                    Name = dto.Name,
                    Address = dto.Address,
                    Phone = dto.Phone
                });
            }

            if (!string.IsNullOrEmpty(name))
            {
                models = models.FindAll(f => f.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            return models;
        }

        public async Task<Franchise> GetOneFranchiseAsync(int id)
        {
            GetFranchiseDTO dto = await _repository.GetOneFranchiseAsync(id);
            if (dto == null) return null;

            return new Franchise
            {
                Franchise_id = dto.Franchise_id,
                Name = dto.Name,
                Address = dto.Address,
                Phone = dto.Phone
            };
        }

        public async Task<bool> AddFranchiseAsync(AddFranchiseDTO dto)
        {
            return await _repository.AddFranchiseAsync(dto);
        }

        public async Task<bool> UpdateFranchiseAsync(UpdateFranchiseDTO dto)
        {
            return await _repository.UpdateFranchiseAsync(dto);
        }

        public async Task<bool> DeleteFranchiseAsync(int id)
        {
            return await _repository.DeleteFranchiseAsync(id);
        }
    }
}