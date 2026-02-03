using System;

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
        public required string Full_name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; } 
        public required string Role { get; set; }
        public int? Franchise_id { get; set; }
    }

    public class UpdateUserDTO
    {
        public required int User_id { get; set; }
        public string? Full_name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public int? Franchise_id { get; set; }
        public string? Password { get; set; }
    }
}