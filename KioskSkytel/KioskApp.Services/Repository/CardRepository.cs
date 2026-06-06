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

        public async Task<List<Card>> GetCardsAsync(int cardType)
        {
            var cards = new List<Card>();

            using (var connection = await _databaseService.GetOpenConnectionAsync())
            {
                var query = "SELECT t1.id, t1.price, t1.duration, t1.unit_amount, t1.data_gb, t1.category, t1.title " +
                    "FROM cards AS t1 " +
                    "WHERE t1.is_active = 1 AND t1.category = @cardCategory";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@cardCategory", cardType);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            cards.Add(new Card
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Price = reader.GetDouble(reader.GetOrdinal("price")),
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                Duration = reader.GetString(reader.GetOrdinal("duration")),
                                UnitAmount = reader.GetInt32(reader.GetOrdinal("unit_amount")),
                                DataGB = reader.GetInt32(reader.GetOrdinal("data_gb")),
                                CardType = reader.GetInt32(reader.GetOrdinal("category")),
                            });
                        }
                    }
                }

                return cards;
            }

        }

        public async Task<List<Card>> GetAllCardsAsync()
        {
            var cards = new List<Card>();

            using (var connection = await _databaseService.GetOpenConnectionAsync())
            {
                var query = "SELECT t1.id, t1.price, t1.duration, t1.unit_amount, t1.data_gb, t1.category, t1.title " +
                    "FROM cards AS t1 " +
                    "WHERE t1.is_active = 1";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            cards.Add(new Card
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Price = reader.GetDouble(reader.GetOrdinal("price")),
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                Duration = reader.GetString(reader.GetOrdinal("duration")),
                                UnitAmount = reader.GetInt32(reader.GetOrdinal("unit_amount")),
                                DataGB = reader.GetInt32(reader.GetOrdinal("data_gb")),
                                CardType = reader.GetInt32(reader.GetOrdinal("category")),
                            });
                        }
                    }
                }

                return cards;
            }
        }
    }
}