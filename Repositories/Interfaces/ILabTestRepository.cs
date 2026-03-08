using SaviaVetAPI.DTOs;

namespace SaviaVetAPI.Repositories
{
    public interface ILabTestRepository
    {
        public Task<List<GetLabTestDTO>> GetLabTestsAsync();
        public Task<List<GetLabTestDTO>> GetLabTestsByOwnerIdAsync(int ownerId);
        public Task<GetLabTestDTO> GetOneLabTestAsync(int id);
        public Task<bool> AddLabTestAsync(AddLabTestDTO addLabTestDTO);
        public Task<bool> UpdateLabTestAsync(UpdateLabTestDTO updateLabTestDTO);
        public Task<bool> DeleteLabTestAsync(int id);
    }
}
