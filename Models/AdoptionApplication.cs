namespace SaviaVetAPI.Models
{
    public class AdoptionApplication
    {
        public int Application_id { get; set; }
        public int User_id { get; set; }
        public int Pet_id { get; set; }
        public string? Message { get; set; } = "";
        public string Status { get; set; } = ""; 
        public DateTime Application_date { get; set; } = DateTime.Now;

        public User? User { get; set; }
        public Pet? Pet { get; set; }

        public AdoptionApplication() { }
    }
}