using System.Windows;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using KioskApp.Services;
using KioskApp.Services.Database;
using KioskSkytel.KioskApp.UI.ViewModels;

namespace KioskSkytel.KioskApp.UI.Views
{
    public partial class BuyCard : Window
    {
        private readonly BuyCardViewModel _viewModel;

        public BuyCard()
        {
            InitializeComponent();

            _viewModel = new BuyCardViewModel();
            _viewModel.MessageRequested += ViewModel_MessageRequested;
            _viewModel.RequestClose += ViewModel_RequestClose;

            DataContext = _viewModel;
            Loaded += BuyCard_Loaded;
        }

        private async void BuyCard_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var connectionString = await GetDatabaseConnectionStringAsync();
                var repository = new CardRepository(new DatabaseService(connectionString));
                await _viewModel.LoadCardsAsync(repository);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Картын мэдээллийг уншиж байх үед алдаа гарлаа: {ex.Message}", "Алдаа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewModel_MessageRequested(object? sender, MessageRequestedEventArgs e)
        {
            MessageBox.Show(e.Message, e.Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ViewModel_RequestClose(object? sender, System.EventArgs e)
        {
            Close();
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
    }
}
