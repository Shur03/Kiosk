namespace KioskApp.Models
{
    public enum PaymentMethodType
    {
        QPAY = 1,
        CREDIT = 2
    }
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}