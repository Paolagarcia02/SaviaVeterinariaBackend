namespace SaviaVetAPI.Models
{
    public class User
    {
        public int User_id { get; set; }
        public string Full_name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password_hash { get; set; } = "";
        public string Role { get; set; } = ""; 
        public int? Franchise_id { get; set; }
        public DateTime Created_at { get; set; } = DateTime.Now;

        public Franchise? Franchise { get; set; }

        public User() { }
    }
}