using KioskApp.Models;
using KioskApp.Services.Database;
using KioskApp.Services.Repository;
using KioskSkytel.KioskApp.UI.Views;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static KioskApp.Models.Card;

namespace KioskSkytel.KioskApp.Views
{
    public partial class GP_Login : Window
    {
        public string? AccountNumber { get; private set; }
        private const int MaxLength = 8;
        private readonly ServiceType _serviceType = ServiceType.GOPLUS;
        private readonly CardCategory _cardType = CardCategory.GOPLUS;

        public GP_Login()
        {
            InitializeComponent();
        }

        private void Key_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
                InputTextBox.Text += btn.Content?.ToString();
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (InputTextBox.Text.Length > 0)
                InputTextBox.Text = InputTextBox.Text[..^1];
        }

        private void MicButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: integrate speech recognition
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InputTextBox.Focus();
            InputTextBox.CaretIndex = InputTextBox.Text.Length;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ContinueButton_Click(sender, e);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Back)
            {
                if (!string.IsNullOrEmpty(InputTextBox.Text))
                {
                    InputTextBox.Text = InputTextBox.Text[..^1];
                    InputTextBox.CaretIndex = InputTextBox.Text.Length;
                }
                e.Handled = true;
                return;
            }

            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                InputTextBox.Text += e.Key.ToString();
                InputTextBox.CaretIndex = InputTextBox.Text.Length;
                e.Handled = true;
                return;
            }

            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                    return;

                var digit = ((int)(e.Key - Key.D0)).ToString();
                InputTextBox.Text += digit;
                InputTextBox.CaretIndex = InputTextBox.Text.Length;
                e.Handled = true;
                return;
            }

            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                var digit = ((int)(e.Key - Key.NumPad0)).ToString();
                InputTextBox.Text += digit;
                InputTextBox.CaretIndex = InputTextBox.Text.Length;
                e.Handled = true;
                return;
            }

            if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                InputTextBox.Text += "_";
                InputTextBox.CaretIndex = InputTextBox.Text.Length;
                e.Handled = true;
                return;
            }

            if (e.Key == Key.OemPeriod)
            {
                InputTextBox.Text += ".";
                InputTextBox.CaretIndex = InputTextBox.Text.Length;
                e.Handled = true;
                return;
            }

            if (e.Key == Key.D2 && Keyboard.Modifiers == ModifierKeys.Shift)
            {
                InputTextBox.Text += "@";
                InputTextBox.CaretIndex = InputTextBox.Text.Length;
                e.Handled = true;
                return;
            }
        }

        private async void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            var accountIdentifier = InputTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(accountIdentifier))
            {
                CustomMessageBox.Show("Нэвтрэх нэрээ оруулна уу.", MessageBoxType.Warning);
                return;
            }

            var dbService = CreateDatabaseService();
            var accountRepo = new AccountRepository(dbService);
            var accounts = await accountRepo.GetAccountsAsync(accountIdentifier, (int)_serviceType);
            if (accounts == null || accounts.Count == 0)
            {
                CustomMessageBox.Show("Бүртгэл олдсонгүй", MessageBoxType.Warning);
                return;
            }

            AccountNumber = accountIdentifier;
            var window = new BuyCard(_cardType)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            Hide();
            window.ShowDialog();
            Close();
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