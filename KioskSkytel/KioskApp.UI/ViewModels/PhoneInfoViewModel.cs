using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using KioskApp.Services.Database;
using KioskApp.Services.Repository;
using KioskApp.Models;

namespace KioskSkytel.KioskApp.UI.ViewModels
{
    public class PhoneInfoViewModel : INotifyPropertyChanged
    {
        private string _bundleName = string.Empty;
        private string _fullName = string.Empty;
        private readonly ServiceType _serviceType = ServiceType.SKYTEL;

        public string AccountNumber { get; }

        public string BundleName
        {
            get => _bundleName;
            private set
            {
                if (_bundleName == value) return;
                _bundleName = value;
                OnPropertyChanged();
            }
        }

        public string FullName
        {
            get => _fullName;
            private set
            {
                if (_fullName == value) return;
                _fullName = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public PhoneInfoViewModel(string accountNumber, ServiceType serviceType = ServiceType.SKYTEL)
        {
            AccountNumber = accountNumber;
            _serviceType = serviceType;
        }

        public async Task LoadAsync()
        {
            var dbService = CreateDatabaseService();

            // get accounts by account number
            var accountRepo = new AccountRepository(dbService);
            var accounts = await accountRepo.GetAccountsAsync(AccountNumber, (int)_serviceType);

            if (accounts?.Count > 0)
            {
                var account = accounts[0];
                BundleName = account.BundleName ?? account.AccountNumber ?? string.Empty;
                FullName = string.IsNullOrWhiteSpace(account.FullName)
                    ? "Мэдээлэл олдсонгүй"
                    : account.FullName;
            }
            else
            {
                BundleName = "Олдсонгүй";
                FullName = "Мэдээлэл олдсонгүй";
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
