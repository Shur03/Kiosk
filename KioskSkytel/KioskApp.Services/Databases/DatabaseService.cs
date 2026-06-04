using Npgsql;
using System.Diagnostics;

namespace KioskApp.Services.Database
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<NpgsqlConnection> GetOpenConnectionAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                Debug.WriteLine("Database-д холбогдож байна...");
                Debug.WriteLine($"Connection String: {_connectionString}");

                // Try to parse the connection string to surface any immediate issues
                try
                {
                    var builder = new NpgsqlConnectionStringBuilder(_connectionString);
                    Debug.WriteLine($"Parsed Host: {builder.Host}, Database: {builder.Database}, Username: {builder.Username}");
                }
                catch (Exception parseEx)
                {
                    Debug.WriteLine($"❌ Connection string parse error: {parseEx.Message}");
                    return false;
                }

                await using (var connection = new NpgsqlConnection(_connectionString))
                {
                    // Open the connection
                    await connection.OpenAsync();

                    // Execute a simple validation query to ensure the connection can run commands
                    await using (var cmd = new NpgsqlCommand("SELECT 1", connection))
                    {
                        cmd.CommandTimeout = 5; // short timeout for test
                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null && Convert.ToInt32(result) == 1)
                        {
                            Debug.WriteLine("✅ Амжилттай холбогдлоо and query succeeded!");
                            return true;
                        }
                        else
                        {
                            Debug.WriteLine("❌ Холболт болсон ч тест query амжилтгүй боллоо.");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Алдаа: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);
                return false;
            }
        }
    }
}