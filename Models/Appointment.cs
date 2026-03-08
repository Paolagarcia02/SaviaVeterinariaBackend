namespace SaviaVetAPI.Models
{
    public class Appointment
    {
        public int Appointment_id { get; set; }
        public DateTime? Date_time { get; set; }
        public int Duration_minutes { get; set; } = 30;
        public string Reason { get; set; } = "";
        public string Status { get; set; } = ""; 
        public string? Notes { get; set; } = "";
        public int Pet_id { get; set; }
        public int? Vet_id { get; set; }
        public int Franchise_id { get; set; }
        public int? Room_id { get; set; }

        public Pet? Pet { get; set; }
        public User? Vet { get; set; }
        public Franchise? Franchise { get; set; }
        public ClinicRoom? Room { get; set; }

        public Appointment() { }
    }
}
