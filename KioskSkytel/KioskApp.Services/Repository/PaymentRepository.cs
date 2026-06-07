using Npgsql;
using KioskApp.Services.Database;
using KioskApp.Models;
namespace KioskApp.Services.Repository
{
    public class PaymentRepository
    {
        private readonly DatabaseService _databaseService;

        public PaymentRepository(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<int?> GetAccountIdByNumberAsync(string accountNumber, int serviceType)
        {
            using var connection = await _databaseService.GetOpenConnectionAsync();
            const string sql = "SELECT id FROM public.accounts WHERE account_number = @accountNumber AND service_type = @serviceType AND is_active = 1 LIMIT 1";

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@accountNumber", accountNumber);
            command.Parameters.AddWithValue("@serviceType", serviceType);

            var result = await command.ExecuteScalarAsync();
            return result == null || result == DBNull.Value ? null : Convert.ToInt32(result);
        }

        public async Task<int> CreateInvoiceAsync(int accountId, decimal amount, DateTime billingPeriod, int status, DateTime dueDate, DateTime createdAt)
        {
            using var connection = await _databaseService.GetOpenConnectionAsync();
            const string sql = @"INSERT INTO public.payment_invoices
                (account_id, billing_period, amount, paid_amount, status, due_date, created_at)
                VALUES (@accountId, @billingPeriod, @amount, @paidAmount, @status, @dueDate,@createdAt)
                RETURNING id";

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@accountId", accountId);
            command.Parameters.AddWithValue("@billingPeriod", billingPeriod);
            command.Parameters.AddWithValue("@amount", amount);
            command.Parameters.AddWithValue("@paidAmount", amount);
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@dueDate", dueDate);
            command.Parameters.AddWithValue("@createdAt", createdAt);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<int> CreatePaymentTransactionAsync(int invoiceId, int accountId, decimal amount, PaymentMethodType paymentMethod, string transactionReference, int status, DateTime createdAt)
        {
            using var connection = await _databaseService.GetOpenConnectionAsync();
            const string sql = @"INSERT INTO public.payment_transactions
                (invoice_id, account_id, amount, payment_method, transaction_reference, status, created_at)
                VALUES (@invoiceId, @accountId, @amount, @paymentMethod, @transactionReference, @status, @createdAt)
                RETURNING id";

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("@invoiceId", invoiceId);
            command.Parameters.AddWithValue("@accountId", accountId);
            command.Parameters.AddWithValue("@amount", amount);
            command.Parameters.AddWithValue("@paymentMethod", (int)paymentMethod);
            command.Parameters.AddWithValue("@transactionReference", transactionReference);
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue("@createdAt", createdAt);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public static string GenerateTransactionReference(PaymentMethodType paymentMethod)
        {
            return $"TX-{paymentMethod}-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant()}";
        }
    }
}
