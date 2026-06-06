using KioskApp.Models;
using KioskApp.Services;
using KioskApp.Services.Database;
using KioskSkytel.KioskApp.UI.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using static KioskApp.Models.Card;

namespace KioskSkytel.KioskApp.UI.ViewModels
{
    public class CardItemViewModel : INotifyPropertyChanged
    {
        private bool _isSelected;

        public CardItemViewModel(Card card)
        {
            Card = card;
        }

        public Card Card { get; }

        public string Name => Card.Title;
        public string Days => Card.Duration;
        public string Units => Card.UnitAmount.ToString();
        public string PriceFormatted => $"{Card.Price:N0}₮";

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BuyCardViewModel : INotifyPropertyChanged
    {
        private readonly CardCategory _cardType;
        private CardItemViewModel? _selectedCard;

        public ObservableCollection<CardItemViewModel> Cards { get; } = new();

        public CardItemViewModel? SelectedCard
        {
            get => _selectedCard;
            private set
            {
                if (_selectedCard == value) return;
                _selectedCard = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectCardCommand { get; }
        public ICommand ContinueCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler? ContinueRequested;

        public BuyCardViewModel(CardCategory cardType = CardCategory.SKYTEL)
        {
            _cardType = cardType;
            SelectCardCommand = new RelayCommand(OnSelectCard);
            ContinueCommand = new RelayCommand(
                _ => ContinueRequested?.Invoke(this, EventArgs.Empty),
                _ => SelectedCard != null);
        }

        public async Task LoadAsync()
        {
            var dbService = CreateDatabaseService();
            var cardRepo = new CardRepository(dbService);
            var cards = await cardRepo.GetCardsAsync((int)_cardType);

            Cards.Clear();
            SelectedCard = null;

            if (cards?.Count > 0)
            {
                foreach (var card in cards)
                {
                    Cards.Add(new CardItemViewModel(card));
                }
            }
        }

        private void OnSelectCard(object? parameter)
        {
            if (parameter is not CardItemViewModel item)
                return;

            foreach (var card in Cards)
            {
                card.IsSelected = false;
            }

            item.IsSelected = true;
            SelectedCard = item;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static DatabaseService CreateDatabaseService()
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
