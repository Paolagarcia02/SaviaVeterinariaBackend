using System;

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
        public required string Name { get; set; }
        public required string Address { get; set; }
        public string? Phone { get; set; }
    }

    public class UpdateFranchiseDTO
    {
        public required int Franchise_id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
    }
}