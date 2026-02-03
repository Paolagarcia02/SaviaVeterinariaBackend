namespace SaviaVetAPI.Models
{
    public class LabTest
    {
        public int Test_id { get; set; }
        public int Appointment_id { get; set; }
        public string Test_type { get; set; } = ""; 
        public string? Result_data { get; set; } = "";
        public string? Comments { get; set; } = "";
        public string Status { get; set; } = ""; 
        public DateTime Requested_at { get; set; } = DateTime.Now;
        public DateTime? Completed_at { get; set; } 

        public Appointment? Appointment { get; set; }

        public LabTest() { }
    }
}