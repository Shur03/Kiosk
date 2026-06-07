namespace KioskApp.Models
{
    public class PaymentInvoice
    {
        public int Id { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public int InvoiceNumber { get; set; } = 0;
        public DateTime BillingPeriod { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public int Status { get; set; } = 0;
        public DateTime DueDate { get; set; }
        public DateTime CreadedAt { get; set; } = DateTime.Now;
    }
}