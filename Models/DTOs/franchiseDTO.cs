using System;
using System.ComponentModel.DataAnnotations;

namespace SaviaVetAPI.DTOs 
{
    public class GetFranchiseDTO
    {
        public int Franchise_id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string? Phone { get; set; }
    }

    public class AddFranchiseDTO
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Address { get; set; }
        public string? Phone { get; set; }
    }

    public class UpdateFranchiseDTO
    {
        [Range(1, int.MaxValue)]
        public required int Franchise_id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}
