using System.Security.Claims;
using SaviaVetAPI.DTOs; 

namespace SaviaVetAPI.Services
{
    public interface IAuthService
    {
        Task<string> Login(LoginDtoIn loginDtoIn);
        
        Task<string> Register(UserDtoIn userDtoIn);
        
        string GenerateToken(UserDtoOut userDtoOut);
        
        bool HasAccessToResource(int requestedUserID, ClaimsPrincipal user);
    }
}