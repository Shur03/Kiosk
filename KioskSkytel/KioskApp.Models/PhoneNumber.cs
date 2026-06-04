namespace KioskApp.Models
{
    public class PhoneNumber
    {
        private const string GOLDEN = "GOLDEN";
        private const string SILVER = "SILVER";
        private const string PLATINUM = "PLATINUM";
        public required string Id { get; set; }
        public required string Number { get; set; }
        public required string Category { get; set; }
    }
}