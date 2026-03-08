namespace SaviaVetAPI.Models
{
    public class ClinicSchedule
    {
        public int Schedule_id { get; set; }
        public int Franchise_id { get; set; }
        public int? Room_id { get; set; }
        public int Day_of_week { get; set; } // 0=Sunday ... 6=Saturday
        public TimeSpan Open_time { get; set; }
        public TimeSpan Close_time { get; set; }
        public bool Is_open { get; set; }
    }
}
