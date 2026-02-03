using System;

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
        public required int Franchise_id { get; set; }
        public required string Name { get; set; }
        public required string Room_type { get; set; }
    }

    public class UpdateClinicRoomDTO
    {
        public required int Room_id { get; set; }
        public string? Name { get; set; }
        public string? Room_type { get; set; }
        public bool? Is_active { get; set; }
    }
}