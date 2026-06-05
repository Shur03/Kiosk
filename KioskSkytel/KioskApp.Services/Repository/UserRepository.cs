using Npgsql;
using KioskApp.Models;
using System.Data;
using KioskApp.Services.Database;

namespace KioskApp.Services
{
    public class UserRepository
    {
        private readonly DatabaseService _databaseService;

        public UserRepository(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = new List<User>();

            using (var connection = await _databaseService.GetOpenConnectionAsync())
            {
                var query = "SELECT id, first_name, last_name, register_number FROM users";

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("id")),
                            FirstName = reader["first_name"].ToString(),
                            LastName = reader["last_name"].ToString(),
                            RegisterNumber = reader["register_number"].ToString()
                        });
                    }
                }
            }

            return users;
        }
    }
}