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
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public string TransactionReference { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}