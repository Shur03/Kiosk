using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Text.Json;
using Npgsql;
using KioskApp.Services.Database;

namespace KioskSkytel.KioskApp.UI.Views
{
    /// <summary>
    /// Interaction logic for PaymentNew.xaml
    /// </summary>
    public partial class PaymentNew : Window
    {
        public event Action? OnClose;
        public string? PhoneNumber { get; private set; }
        private string _input = "";
        private const int MaxLength = 8;
        public PaymentNew()
        {
            InitializeComponent();

            Loaded += (_, __) =>
            {
                Keyboard.Focus(this);
            };
        }
        private void NumBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_input.Length >= MaxLength) return;
            _input += ((Button)sender).Tag.ToString();
            TxtDisplay.Text = _input;
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_input.Length == 0) return;
            _input = _input[..^1];
            TxtDisplay.Text = _input;
        }

        private async void OkBtn_Click(object sender, RoutedEventArgs? e)
        {
            //Close();
            if (_input.Length != MaxLength)
            {
                MessageBox.Show("Оруулсан дугаар буруу байна.");
                return;
            }

            PhoneNumber = _input;

            bool exists = false;
            try
            {
                exists = await PhoneNumberExistsAsync(PhoneNumber);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Алдаа шалгах үед: {ex.Message}", "Алдаа", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!exists)
            {
                MessageBox.Show("Оруулсан дугаар буруу байна.");
                Console.WriteLine("Алдаа", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new PhoneInfo(PhoneNumber)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            Hide();
            window.ShowDialog();
            Close();
        }

        private async Task<bool> PhoneNumberExistsAsync(string? number)
        {
            if (string.IsNullOrWhiteSpace(number)) return false;

            // Try to locate appsettings.json in the application base directory
            string basePath = AppDomain.CurrentDomain.BaseDirectory ?? Directory.GetCurrentDirectory();
            string settingsPath = System.IO.Path.Combine(basePath, "appsettings.json");
            if (!File.Exists(settingsPath))
            {
                // fallback to project folder
                settingsPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "KioskApp.UI", "appsettings.json");
            }

            if (!File.Exists(settingsPath))
                throw new FileNotFoundException("Холболт амжилтгүй", settingsPath);

            using var fs = File.OpenRead(settingsPath);
            using var doc = await JsonDocument.ParseAsync(fs);
            var root = doc.RootElement;
            if (!root.TryGetProperty("Database", out var db))
                throw new InvalidDataException("Холболт амжилтгүй(db)");

            string host = db.GetProperty("Host").GetString() ?? "localhost";
            string port = db.GetProperty("Port").GetString() ?? "5432";
            string name = db.GetProperty("Name").GetString() ?? string.Empty;
            string username = db.GetProperty("Username").GetString() ?? string.Empty;
            string password = db.TryGetProperty("Password", out var pw) ? pw.GetString() ?? string.Empty : string.Empty;

            // Include password and disable SSL for local/testing if needed. Adjust Ssl Mode per your environment.
            var connStr = $"Host={host};Port={port};Username={username};Password={password};Database={name};Ssl Mode=Disable;";

            var dbService = new DatabaseService(connStr);
            await using var conn = await dbService.GetOpenConnectionAsync();

            var sql = "SELECT 1 FROM public.phone_numbers WHERE phone_number = @p LIMIT 1";
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@p", number);
            var res = await cmd.ExecuteScalarAsync();
            return res != null;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs? e)
        {
            DialogResult = false;
            Close();
        }

        private void NumPadGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double btnSize = (NumPadGrid.ActualWidth - 48) / 3;
            foreach (var row in NumPadGrid.RowDefinitions)
                row.Height = new GridLength(btnSize);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Top row numbers
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                string digit = ((int)(e.Key - Key.D0)).ToString();
                AppendDigit(digit);
            }

            // Numpad numbers
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                string digit = ((int)(e.Key - Key.NumPad0)).ToString();
                AppendDigit(digit);
            }

            // Backspace
            else if (e.Key == Key.Back)
            {
                ClearLastDigit();
            }

            // Enter
            else if (e.Key == Key.Enter)
            {
                OkBtn_Click(this, null);
            }

            // Esc
            else if (e.Key == Key.Escape)
            {
                CancelBtn_Click(this, null);
            }
        }

        private void AppendDigit(string digit)
        {
            if (_input.Length >= MaxLength) return;
            _input += digit;
            TxtDisplay.Text = _input;
        }

        private void ClearLastDigit()
        {
            if (_input.Length == 0) return;
            _input = _input[..^1];
            TxtDisplay.Text = _input;
        }
    }
}
