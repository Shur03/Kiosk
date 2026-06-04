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
                            Id = reader["id"].ToString(),
                            FirstName = reader["first_name"].ToString(),
                            LastName = reader["last_name"].ToString(),
                            RegisterNumber = reader["register_number"].ToString()
                        });
                    }
                }
            }

            return users;
        }

        public async Task<int> InsertUserAsync(User user)
        {
            using (var connection = await _databaseService.GetOpenConnectionAsync())
            {
                var query = @"
                    INSERT INTO users (id, first_name, last_name, register_number) 
                    VALUES (@id, @firstName, @lastName, @registerNumber)";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@firstName", user.FirstName);
                    command.Parameters.AddWithValue("@lastName", user.LastName);
                    command.Parameters.AddWithValue("@registerNumber", user.RegisterNumber);

                    return await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}