using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using KioskApp.Models;
using KioskApp.Services;
using KioskApp.Services.Database;
using KioskSkytel.KioskApp.UI.Helpers;

namespace KioskSkytel.KioskApp.UI.Views
{
    public partial class BuyCard : Window
    {
        public ObservableCollection<Card> Cards { get; } = new();
        public ICommand BuyCommand { get; }

        public BuyCard()
        {
            InitializeComponent();
            BuyCommand = new RelayCommand(p => OnBuy(p));
            DataContext = this;
            Loaded += BuyCard_Loaded;
        }

        private async void BuyCard_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadCardsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Картын мэдээллийг уншиж байх үед алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadCardsAsync()
        {
            var connectionString = await GetDatabaseConnectionStringAsync();
            var repository = new CardRepository(new DatabaseService(connectionString));
            var cards = await repository.GetAllCardsAsync();

            Cards.Clear();
            foreach (var card in cards)
            {
                Cards.Add(card);
            }
        }

        private static async Task<string> GetDatabaseConnectionStringAsync()
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
            using var document = await JsonDocument.ParseAsync(fs);
            var root = document.RootElement;
            if (!root.TryGetProperty("Database", out var databaseSection))
                throw new InvalidDataException("appsettings.json-д Database хэсэг байхгүй байна.");

            string host = databaseSection.GetProperty("Host").GetString() ?? "localhost";
            string port = databaseSection.GetProperty("Port").GetString() ?? "5432";
            string name = databaseSection.GetProperty("Name").GetString() ?? string.Empty;
            string username = databaseSection.GetProperty("Username").GetString() ?? string.Empty;
            string password = databaseSection.TryGetProperty("Password", out var pw) ? pw.GetString() ?? string.Empty : string.Empty;

            return $"Host={host};Port={port};Username={username};Password={password};Database={name};Ssl Mode=Disable;";
        }

        private void OnBuy(object? param)
        {
            if (param is Card pkg)
            {
                MessageBox.Show($"Сонгосон багц: {pkg.Price} - {pkg.DataGB}GB / {pkg.Duration}", "Худалдан авах", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
