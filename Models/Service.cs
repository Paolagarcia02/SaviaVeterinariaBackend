namespace SaviaVetAPI.Models
{
    public class Service
    {
        public int Service_id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string? Icon { get; set; } = "";
        public DateTime Created_at { get; set; } = DateTime.Now;

        public Service() { }
    }
}