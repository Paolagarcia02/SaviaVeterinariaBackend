using SaviaVetAPI.DTOs;

namespace SaviaVetAPI.Repositories
{
    public interface IAdoptionApplicationRepository
    {
        public Task<List<GetAdoptionApplicationDTO>> GetAdoptionApplicationsAsync();
        public Task<GetAdoptionApplicationDTO> GetOneAdoptionApplicationAsync(int id);
        public Task<bool> AddAdoptionApplicationAsync(AddAdoptionApplicationDTO addAdoptionApplicationDTO);
        public Task<bool> UpdateAdoptionApplicationAsync(UpdateAdoptionApplicationDTO updateAdoptionApplicationDTO);
        public Task<bool> DeleteAdoptionApplicationAsync(int id);
    }
}