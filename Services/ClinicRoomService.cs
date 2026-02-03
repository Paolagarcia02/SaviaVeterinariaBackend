using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;
using SaviaVetAPI.Repositories;

namespace SaviaVetAPI.Services
{
    public class ClinicRoomService : IClinicRoomService
    {
        private readonly IClinicRoomRepository _repository;

        public ClinicRoomService(IClinicRoomRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ClinicRoom>> GetClinicRoomsAsync(int? franchiseId, string? roomType)
        {
            List<GetClinicRoomDTO> dtos = await _repository.GetClinicRoomsAsync();

            List<ClinicRoom> models = new List<ClinicRoom>();
            foreach (var dto in dtos)
            {
                models.Add(new ClinicRoom
                {
                    Room_id = dto.Room_id,
                    Franchise_id = dto.Franchise_id,
                    Name = dto.Name,
                    Room_type = dto.Room_type,
                    Is_active = dto.Is_active
                });
            }

            if (franchiseId != null)
            {
                models = models.FindAll(r => r.Franchise_id == franchiseId);
            }
            if (!string.IsNullOrEmpty(roomType))
            {
                models = models.FindAll(r => r.Room_type.Contains(roomType));
            }

            return models;
        }

        public async Task<ClinicRoom> GetOneClinicRoomAsync(int id)
        {
            GetClinicRoomDTO dto = await _repository.GetOneClinicRoomAsync(id);
            if (dto == null) return null;

            return new ClinicRoom
            {
                Room_id = dto.Room_id,
                Franchise_id = dto.Franchise_id,
                Name = dto.Name,
                Room_type = dto.Room_type,
                Is_active = dto.Is_active
            };
        }

        public async Task<bool> AddClinicRoomAsync(AddClinicRoomDTO dto)
        {
            return await _repository.AddClinicRoomAsync(dto);
        }

        public async Task<bool> UpdateClinicRoomAsync(UpdateClinicRoomDTO dto)
        {
            return await _repository.UpdateClinicRoomAsync(dto);
        }

        public async Task<bool> DeleteClinicRoomAsync(int id)
        {
            return await _repository.DeleteClinicRoomAsync(id);
        }
    }
}