using System;

namespace SaviaVetAPI.DTOs
{
    public class GetLabTestDTO
    {
        public int Test_id { get; set; }
        public int Appointment_id { get; set; }
        public string Test_type { get; set; } = "";
        public string? Result_data { get; set; }
        public string? Comments { get; set; }
        public string Status { get; set; } = "";
        public DateTime Requested_at { get; set; }
        public DateTime? Completed_at { get; set; }
    }

    public class AddLabTestDTO
    {
        public required int Appointment_id { get; set; }
        public required string Test_type { get; set; }
    }

    public class UpdateLabTestDTO
    {
        public required int Test_id { get; set; }
        public string? Result_data { get; set; }
        public string? Comments { get; set; }
        public string? Status { get; set; }
        public DateTime? Completed_at { get; set; }
    }
}