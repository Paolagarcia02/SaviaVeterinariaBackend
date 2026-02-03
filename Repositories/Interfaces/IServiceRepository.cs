using SaviaVetAPI.DTOs;

namespace SaviaVetAPI.Repositories
{
    public interface IServiceRepository
    {
        public Task<List<GetServiceDTO>> GetServicesAsync();
        public Task<GetServiceDTO> GetOneServiceAsync(int id);
        public Task<bool> AddServiceAsync(AddServiceDTO addServiceDTO);
        public Task<bool> UpdateServiceAsync(UpdateServiceDTO updateServiceDTO);
        public Task<bool> DeleteServiceAsync(int id);
    }
}