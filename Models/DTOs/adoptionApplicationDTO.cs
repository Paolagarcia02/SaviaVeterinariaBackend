using System;
using System.ComponentModel.DataAnnotations;

namespace SaviaVetAPI.DTOs
{
    public class GetAdoptionApplicationDTO
    {
        public int Application_id { get; set; }
        public int User_id { get; set; }
        public int Pet_id { get; set; }
        public string? Message { get; set; }
        public string Status { get; set; } = "";
        public DateTime Application_date { get; set; }
    }

    public class AddAdoptionApplicationDTO
    {
        [Range(1, int.MaxValue)]
        public required int User_id { get; set; }
        [Range(1, int.MaxValue)]
        public required int Pet_id { get; set; }
        public string? Message { get; set; }
        public string? Status { get; set; }
    }

    public class UpdateAdoptionApplicationDTO
    {
        [Range(1, int.MaxValue)]
        public required int Application_id { get; set; }
        public string? Status { get; set; } 
    }
}
