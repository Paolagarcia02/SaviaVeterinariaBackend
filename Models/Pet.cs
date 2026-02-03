namespace SaviaVetAPI.Models
{
    public class Pet
    {
        public int Pet_id { get; set; }
        public string Name { get; set; } = "";
        public string Species { get; set; } = "";
        public string? Breed { get; set; }
        public DateTime? Birth_date { get; set; }
        public string? Photo_url { get; set; } = "";
        public string? Description { get; set; } = "";
        public string Status { get; set; } = ""; 
        public int? Owner_id { get; set; }
        public DateTime Created_at { get; set; } = DateTime.Now;

        public User? Owner { get; set; }

        public Pet() { }
    }
}