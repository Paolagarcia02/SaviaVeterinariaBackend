using System.ComponentModel.DataAnnotations;

namespace SaviaVetAPI.DTOs
{
    public class GetClinicScheduleDTO
    {
        public int Schedule_id { get; set; }
        public int Franchise_id { get; set; }
        public int? Room_id { get; set; }
        public int Day_of_week { get; set; }
        public TimeSpan Open_time { get; set; }
        public TimeSpan Close_time { get; set; }
        public bool Is_open { get; set; }
    }

    public class AddClinicScheduleDTO
    {
        [Range(1, int.MaxValue)]
        public required int Franchise_id { get; set; }
        public int? Room_id { get; set; }
        [Range(0, 6)]
        public required int Day_of_week { get; set; }
        [Required]
        public required TimeSpan Open_time { get; set; }
        [Required]
        public required TimeSpan Close_time { get; set; }
        public bool Is_open { get; set; } = true;
    }

    public class UpdateClinicScheduleDTO
    {
        [Range(1, int.MaxValue)]
        public required int Schedule_id { get; set; }
        [Range(0, 6)]
        public int? Day_of_week { get; set; }
        public TimeSpan? Open_time { get; set; }
        public TimeSpan? Close_time { get; set; }
        public bool? Is_open { get; set; }
    }
}
