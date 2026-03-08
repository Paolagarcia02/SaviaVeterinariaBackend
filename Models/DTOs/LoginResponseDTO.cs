using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SaviaVetAPI.DTOs;

    public class LoginResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int? FranchiseId { get; set; }
    }
