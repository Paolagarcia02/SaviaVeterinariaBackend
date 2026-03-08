using System;
using System.ComponentModel.DataAnnotations;

namespace SaviaVetAPI.DTOs
{
    public class GetClinicRoomDTO
    {
        public int Room_id { get; set; }
        public int Franchise_id { get; set; }
        public string Name { get; set; } = "";
        public string Room_type { get; set; } = ""; // 'Consulta', 'Quirófano'...
        public bool Is_active { get; set; }
    }

    public class AddClinicRoomDTO
    {
        [Range(1, int.MaxValue)]
        public required int Franchise_id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public required string Room_type { get; set; }
    }

    public class UpdateClinicRoomDTO
    {
        [Range(1, int.MaxValue)]
        public required int Room_id { get; set; }
        public string? Name { get; set; }
        public string? Room_type { get; set; }
        public bool? Is_active { get; set; }
    }
}
