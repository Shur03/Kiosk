namespace KioskApp.Models
{
    public class PaymentInvoice
    {
        public int Id { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public string BillingPeriod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
    }
}