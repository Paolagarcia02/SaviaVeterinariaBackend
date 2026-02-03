using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;
using SaviaVetAPI.Repositories;

namespace SaviaVetAPI.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _repository;

        public PetService(IPetRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Pet>> GetPetsAsync(int? ownerId, string? species)
        {
            List<GetPetDTO> dtos = await _repository.GetPetsAsync();

            List<Pet> models = new List<Pet>();
            foreach (var dto in dtos)
            {
                models.Add(new Pet
                {
                    Pet_id = dto.Pet_id,
                    Name = dto.Name,
                    Species = dto.Species,
                    Breed = dto.Breed,
                    Birth_date = dto.Birth_date,
                    Photo_url = dto.Photo_url,
                    Description = dto.Description,
                    Status = dto.Status,
                    Owner_id = dto.Owner_id
                });
            }

            if (ownerId != null)
            {
                models = models.FindAll(p => p.Owner_id == ownerId);
            }
            if (!string.IsNullOrEmpty(species))
            {
                models = models.FindAll(p => p.Species.Contains(species, StringComparison.OrdinalIgnoreCase));
            }

            return models;
        }

        public async Task<Pet> GetOnePetAsync(int id)
        {
            GetPetDTO dto = await _repository.GetOnePetAsync(id);
            if (dto == null) return null;

            return new Pet
            {
                Pet_id = dto.Pet_id,
                Name = dto.Name,
                Species = dto.Species,
                Breed = dto.Breed,
                Birth_date = dto.Birth_date,
                Photo_url = dto.Photo_url,
                Description = dto.Description,
                Status = dto.Status,
                Owner_id = dto.Owner_id
            };
        }

        public async Task<bool> AddPetAsync(AddPetDTO dto)
        {
            return await _repository.AddPetAsync(dto);
        }

        public async Task<bool> UpdatePetAsync(UpdatePetDTO dto)
        {
            return await _repository.UpdatePetAsync(dto);
        }

        public async Task<bool> DeletePetAsync(int id)
        {
            return await _repository.DeletePetAsync(id);
        }
    }
}