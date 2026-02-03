namespace SaviaVetAPI.Models
{
    public class ClinicRoom
    {
        public int Room_id { get; set; }
        public int Franchise_id { get; set; }
        public string Name { get; set; } = "";
        public string Room_type { get; set; } = ""; 
        public bool Is_active { get; set; }

        public Franchise? Franchise { get; set; }

        public ClinicRoom() { }
    }
}