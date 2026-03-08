using System;
using System.ComponentModel.DataAnnotations;

namespace SaviaVetAPI.DTOs
{
    public class GetServiceDTO
    {
        public int Service_id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string? Icon { get; set; }
    }

    public class AddServiceDTO
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Description { get; set; }
        public string? Icon { get; set; }
    }

    public class UpdateServiceDTO
    {
        [Range(1, int.MaxValue)]
        public required int Service_id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
    }
}
