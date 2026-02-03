using SaviaVetAPI.DTOs;

namespace SaviaVetAPI.Repositories
{
    public interface IFranchiseRepository
    {
        public Task<List<GetFranchiseDTO>> GetFranchisesAsync();
        public Task<GetFranchiseDTO> GetOneFranchiseAsync(int id);
        public Task<bool> AddFranchiseAsync(AddFranchiseDTO addFranchiseDTO);
        public Task<bool> UpdateFranchiseAsync(UpdateFranchiseDTO updateFranchiseDTO);
        public Task<bool> DeleteFranchiseAsync(int id);
    }
}