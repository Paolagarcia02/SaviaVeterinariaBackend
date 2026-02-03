namespace SaviaVetAPI.Models
{
    public class Franchise
    {
        public int Franchise_id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string? Phone { get; set; } = "";
        public DateTime Created_at { get; set; } = DateTime.Now;

        public Franchise() { }
    }
}