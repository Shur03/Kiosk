using Npgsql;
using KioskApp.Models;
using System.Data;
using KioskApp.Services.Database;

namespace KioskApp.Services
{
    public class CardRepository
    {
        private readonly DatabaseService _databaseService;

        public CardRepository(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<Card>> GetAllCardsAsync()
        {
            var cards = new List<Card>();

            using (var connection = await _databaseService.GetOpenConnectionAsync())
            {
                var query = "SELECT price, duration, unit_amount, data_gb FROM cards";

                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        cards.Add(new Card
                        {
                            Price = reader["price"].ToString(),
                            Duration = reader["duration"].ToString(),
                            UnitAmount = reader["unit_amount"].ToString(),
                            DataGB = reader["data_gb"].ToString()
                        });
                    }
                }
            }

            return cards;
        }
    }
}