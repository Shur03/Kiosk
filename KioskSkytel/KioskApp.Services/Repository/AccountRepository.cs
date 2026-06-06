using Npgsql;
using KioskApp.Models;
using System.Data;
using KioskApp.Services.Database;


namespace KioskApp.Services.Repository
{
    public class AccountRepository
    {
        private readonly DatabaseService _databaseService;

        public AccountRepository(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            var accounts = new List<Account>();

            using (var connection = await _databaseService.GetOpenConnectionAsync())
            {
                var query = "SELECT id, user_id, service_type, account_number, is_active, created_at " +
                            "FROM accounts WHERE is_active = 1";

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        accounts.Add(new Account
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                            ServiceType = (ServiceType)reader.GetInt32(reader.GetOrdinal("service_type")),
                            AccountNumber = reader.GetString(reader.GetOrdinal("account_number")),
                            IsActive = reader.GetInt32(reader.GetOrdinal("is_active")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                        });
                    }
                }
            }

            return accounts;
        }

        public async Task<List<Account>> GetAccountsAsync(string accountNumber, int serviceType)
        {
            var accounts = new List<Account>();

            using (var connection = await _databaseService.GetOpenConnectionAsync())
            {
                var query = "SELECT t1.id, t1.user_id, t1.service_type, t1.account_number, t1.is_active, t1.created_at, " +
                    "CONCAT(LEFT(u.first_name, 1),'***',RIGHT(u.first_name, 1)) AS full_name, t3.title AS bundle_name " +
                    "FROM accounts AS t1 " +
                    "LEFT JOIN public.users AS u ON t1.user_id = u.id " +
                    "LEFT JOIN public.account_bundles AS t2 ON t2.account_id = t1.id " +
                    "LEFT JOIN public.bundles AS t3 ON t2.bundle_id = t3.id " +
                    "WHERE t1.is_active = 1 AND t1.account_number = @accountNumber AND t1.service_type = @serviceType";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@accountNumber", accountNumber);
                    command.Parameters.AddWithValue("@serviceType", serviceType);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            accounts.Add(new Account
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                                ServiceType = (ServiceType)reader.GetInt32(reader.GetOrdinal("service_type")),
                                AccountNumber = reader.GetString(reader.GetOrdinal("account_number")),
                                FullName = reader.IsDBNull(reader.GetOrdinal("full_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("full_name")),
                                IsActive = reader.GetInt32(reader.GetOrdinal("is_active")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                BundleName = reader.IsDBNull(reader.GetOrdinal("bundle_name")) ? string.Empty : reader.GetString(reader.GetOrdinal("bundle_name")),
                            });
                        }
                    }
                }

                return accounts;
            }

        }
    }
}