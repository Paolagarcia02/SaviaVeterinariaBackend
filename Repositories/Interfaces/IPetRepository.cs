using SaviaVetAPI.DTOs;

namespace SaviaVetAPI.Repositories
{
    public interface IPetRepository
    {
        public Task<List<GetPetDTO>> GetPetsAsync();
        public Task<GetPetDTO> GetOnePetAsync(int id);
        public Task<bool> AddPetAsync(AddPetDTO addPetDTO);
        public Task<bool> UpdatePetAsync(UpdatePetDTO updatePetDTO);
        public Task<bool> DeletePetAsync(int id);
    }
}