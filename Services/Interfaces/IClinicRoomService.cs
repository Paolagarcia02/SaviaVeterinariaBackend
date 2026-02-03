using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;

namespace SaviaVetAPI.Services
{
    public interface IClinicRoomService
    {
        // Filtros: por franquicia o tipo de sala
        Task<List<ClinicRoom>> GetClinicRoomsAsync(int? franchiseId, string? roomType);
        Task<ClinicRoom> GetOneClinicRoomAsync(int id);
        
        Task<bool> AddClinicRoomAsync(AddClinicRoomDTO dto);
        Task<bool> UpdateClinicRoomAsync(UpdateClinicRoomDTO dto);
        Task<bool> DeleteClinicRoomAsync(int id);
    }
}