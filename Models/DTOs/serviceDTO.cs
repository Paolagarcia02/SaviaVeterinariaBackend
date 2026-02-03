using System;

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
        public required string Name { get; set; }
        public required string Description { get; set; }
        public string? Icon { get; set; }
    }

    public class UpdateServiceDTO
    {
        public required int Service_id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
    }
}