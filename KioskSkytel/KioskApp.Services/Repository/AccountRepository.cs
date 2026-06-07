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

        public async Task<List<Account>> GetAccountsAsync(string accountNumber, int serviceType)
        {
            var accounts = new List<Account>();

            using (var connection = await _databaseService.GetOpenConnectionAsync())
            {
                var query = "SELECT t1.id, t1.user_id, t1.service_type, t1.account_number, t1.is_active, t1.created_at, " +
                    "CONCAT(LEFT(u.first_name, 1),'***',RIGHT(u.first_name, 1), ' ',LEFT(u.last_name, 1),'***',RIGHT(u.last_name, 1) ) AS full_name, t3.title AS bundle_name " +
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

        private static async Task<int> GetOrCreateUserIdByRegisterNumberAsync(NpgsqlConnection connection, string registerNumber, string firstName, string lastName)
        {
            const string selectSql = "SELECT id FROM public.users WHERE register_number = @registerNumber LIMIT 1";
            await using (var selectCommand = new NpgsqlCommand(selectSql, connection))
            {
                selectCommand.Parameters.AddWithValue("@registerNumber", registerNumber);
                var result = await selectCommand.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }

            const string insertSql = "INSERT INTO public.users (first_name, last_name, register_number) VALUES (@firstName, @lastName, @registerNumber) RETURNING id";
            await using (var insertCommand = new NpgsqlCommand(insertSql, connection))
            {
                insertCommand.Parameters.AddWithValue("@firstName", firstName ?? string.Empty);
                insertCommand.Parameters.AddWithValue("@lastName", lastName ?? string.Empty);
                insertCommand.Parameters.AddWithValue("@registerNumber", registerNumber);
                var insertedId = await insertCommand.ExecuteScalarAsync();
                return Convert.ToInt32(insertedId);
            }
        }

        public async Task<int> CreateAccountForRegisterNumberAsync(string accountNumber, string registerNumber, ServiceType serviceType, string? firstName = null, string? lastName = null)
        {
            await using var connection = await _databaseService.GetOpenConnectionAsync();
            var userId = await GetOrCreateUserIdByRegisterNumberAsync(connection, registerNumber, firstName ?? string.Empty, lastName ?? string.Empty);

            const string insertSql = @"INSERT INTO public.accounts (user_id, service_type, account_number, is_active, created_at)
                VALUES (@userId, @serviceType, @accountNumber, @isActive, @createdAt)
                RETURNING id";

            await using var command = new NpgsqlCommand(insertSql, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@serviceType", (int)serviceType);
            command.Parameters.AddWithValue("@accountNumber", accountNumber);
            command.Parameters.AddWithValue("@isActive", 1);
            command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }
}