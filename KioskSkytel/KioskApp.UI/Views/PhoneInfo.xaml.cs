using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KioskApp.Services.Database;
using Npgsql;

namespace KioskSkytel.KioskApp.UI.Views
{
    /// <summary>
    /// Interaction logic for PhoneInfo.xaml
    /// </summary>
    public partial class PhoneInfo : Window
    {
        public string PhoneNumber { get; }
        public string UserName { get; private set; } = string.Empty;
        public string Bundle { get; private set; } = string.Empty;

        public PhoneInfo(string phoneNumber)
        {
            InitializeComponent();
            PhoneNumber = phoneNumber;
            DataContext = this;
            Loaded += PhoneInfo_Loaded;
        }

        private async void PhoneInfo_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPhoneInfoAsync();
        }

        private async Task LoadPhoneInfoAsync()
        {
            try
            {
                var dbService = CreateDatabaseService();
                await using var connection = await dbService.GetOpenConnectionAsync();

                const string sql = @"
SELECT
    LEFT(u.first_name || ' ' || u.last_name, 1)
        || REPEAT(
            '*',
            GREATEST(LENGTH(u.first_name || ' ' || u.last_name) - 2, 0)
        )
        || RIGHT(u.first_name || ' ' || u.last_name, 1) AS user_name_masked,

    pn.phone_number,
    b.title AS bundle_title

FROM public.users u
LEFT JOIN public.user_phones up
    ON up.user_id = u.id
LEFT JOIN public.phone_numbers pn
    ON pn.id = up.phone_number_id
LEFT JOIN public.phone_bundles pb
    ON pb.phone_number_id = pn.id
LEFT JOIN public.bundles b
    ON b.id = pb.bundle_id
WHERE pn.phone_number = @phoneNumber
LIMIT 1;";

                await using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@phoneNumber", PhoneNumber);

                await using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    UserName = reader["user_name_masked"]?.ToString() ?? string.Empty;
                    Bundle = reader["bundle_title"]?.ToString() ?? string.Empty;
                }
                else
                {
                    UserName = "Мэдээлэл олдсонгүй";
                    Bundle = "Олдсонгүй";
                }

                DataContext = null;
                DataContext = this;
            }
            catch (Exception ex)
            {
                UserName = "Алдаа үүслээ";
                Bundle = "Алдаа";
                DataContext = null;
                DataContext = this;
                MessageBox.Show($"PhoneInfo уншиж чадсангүй: {ex.Message}", "Алдаа", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
