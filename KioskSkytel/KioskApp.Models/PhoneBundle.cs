namespace KioskApp.Models
{
    public class PhoneBundle
    {
        public required string Id { get; set; }
        public required string PhoneId { get; set; }
        public required string BundleId { get; set; }
        public required decimal CreatedAt { get; set; }
        public required decimal UpdatedAt { get; set; }
    }
}