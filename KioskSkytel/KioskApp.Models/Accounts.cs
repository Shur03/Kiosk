namespace KioskApp.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}