using KioskApp.Models;
using KioskApp.Services;
using KioskApp.Services.Database;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using static KioskApp.Models.Card;

namespace KioskSkytel.KioskApp.UI.ViewModels
{
    public class GP_CardViewModel : INotifyPropertyChanged
    {
        private readonly CardCategory _cardType = CardCategory.GOPLUS;

        public ObservableCollection<Card> Cards { get; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public GP_CardViewModel(CardCategory cardType = CardCategory.GOPLUS)
        {
            _cardType = cardType;
        }

        public async Task LoadAsync()
        {
            var dbService = CreateDatabaseService();

            var cardRepo = new CardRepository(dbService);
            var cards = await cardRepo.GetCardsAsync((int)_cardType);

            Cards.Clear();
            if (cards?.Count > 0)
            {
                foreach (var card in cards)
                {
                    Cards.Add(card);
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private DatabaseService CreateDatabaseService()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory ?? Directory.GetCurrentDirectory();
            string settingsPath = Path.Combine(basePath, "appsettings.json");
            if (!File.Exists(settingsPath))
            {
                settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "KioskApp.UI", "appsettings.json");
            }
            if (!File.Exists(settingsPath))
                throw new FileNotFoundException("appsettings.json олдсонгүй", settingsPath);

            using var fs = File.OpenRead(settingsPath);
            using var doc = JsonDocument.Parse(fs);
            if (!doc.RootElement.TryGetProperty("Database", out var db))
                throw new InvalidDataException("appsettings.json-д Database хэсэг байхгүй");

            string host = db.GetProperty("Host").GetString() ?? "localhost";
            string port = db.GetProperty("Port").GetString() ?? "5432";
            string name = db.GetProperty("Name").GetString() ?? string.Empty;
            string username = db.GetProperty("Username").GetString() ?? string.Empty;
            string password = db.TryGetProperty("Password", out var pw) ? pw.GetString() ?? string.Empty : string.Empty;

            var connStr = $"Host={host};Port={port};Username={username};Password={password};Database={name};Ssl Mode=Disable;";
            return new DatabaseService(connStr);
        }
    }
}
