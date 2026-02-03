using SaviaVetAPI.DTOs;

namespace SaviaVetAPI.Repositories
{
    public interface IClinicRoomRepository
    {
        public Task<List<GetClinicRoomDTO>> GetClinicRoomsAsync();
        public Task<GetClinicRoomDTO> GetOneClinicRoomAsync(int id);
        public Task<bool> AddClinicRoomAsync(AddClinicRoomDTO addClinicRoomDTO);
        public Task<bool> UpdateClinicRoomAsync(UpdateClinicRoomDTO updateClinicRoomDTO);
        public Task<bool> DeleteClinicRoomAsync(int id);
    }
}