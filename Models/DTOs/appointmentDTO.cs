using System;

namespace SaviaVetAPI.DTOs
{
    public class GetAppointmentDTO
    {
        public int Appointment_id { get; set; }
        public DateTime Date_time { get; set; }
        public int Duration_minutes { get; set; }
        public string Reason { get; set; } = "";
        public string Status { get; set; } = ""; 
        public string? Notes { get; set; }
        public int Pet_id { get; set; }
        public int Vet_id { get; set; }
        public int Franchise_id { get; set; }
        public int? Room_id { get; set; }
    }

    public class AddAppointmentDTO
    {
        public required DateTime Date_time { get; set; }
        public int Duration_minutes { get; set; } = 30; 
        public required string Reason { get; set; }
        public required int Pet_id { get; set; }
        public required int Vet_id { get; set; }
        public required int Franchise_id { get; set; }
        public string? Status { get; set; }
    }

    public class UpdateAppointmentDTO
    {
        public required int Appointment_id { get; set; }
        public DateTime? Date_time { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public int? Vet_id { get; set; }
        public int? Room_id { get; set; } 
    }
}