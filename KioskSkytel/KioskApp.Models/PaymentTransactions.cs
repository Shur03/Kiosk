namespace KioskApp.Models
{
    /*
     id,
    invoice_id,
    account_id,
    amount,
    payment_method,
    transaction_reference,
    status*/
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionReference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}