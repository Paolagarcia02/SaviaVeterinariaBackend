using System;
using System.ComponentModel.DataAnnotations;

namespace SaviaVetAPI.DTOs
{
    public class GetUserDTO
    {
        public int User_id { get; set; }
        public string Full_name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = ""; 
        public int? Franchise_id { get; set; }
    }

    public class AddUserDTO
    {
        [Required]
        public required string Full_name { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; } 
        [Required]
        public required string Role { get; set; }
        public int? Franchise_id { get; set; }
    }

    public class UpdateUserDTO
    {
        [Range(1, int.MaxValue)]
        public required int User_id { get; set; }
        public string? Full_name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Role { get; set; }
        public int? Franchise_id { get; set; }
        public string? Password { get; set; }
    }
}
