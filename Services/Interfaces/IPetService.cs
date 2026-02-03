using SaviaVetAPI.DTOs;
using SaviaVetAPI.Models;

namespace SaviaVetAPI.Services
{
    public interface IPetService
    {
        // Filtros: por dueño, especie o estado
        Task<List<Pet>> GetPetsAsync(int? ownerId, string? species);
        Task<Pet> GetOnePetAsync(int id);
        
        Task<bool> AddPetAsync(AddPetDTO dto);
        Task<bool> UpdatePetAsync(UpdatePetDTO dto);
        Task<bool> DeletePetAsync(int id);
    }
}