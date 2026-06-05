using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using KioskApp.Models;
using KioskApp.Services.Database;
using KioskSkytel.KioskApp.UI.Helpers;
using Npgsql;

namespace KioskSkytel.KioskApp.UI.ViewModels
{
    public class AccountInputViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private const int MaxLengthConst = 8;
        private string _input = string.Empty;
        private readonly ServiceType _serviceType;

        public string Input
        {
            get => _input;
            set
            {
                if (_input == value) return;
                _input = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanAccept));
            }
        }

        public bool CanAccept => Input?.Length == MaxLengthConst;

        public ICommand DigitCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<string, ServiceType>? PhoneInfoRequested;
        public event Action<string, string>? MessageRequested;
        public event Action? CancelRequested;

        public AccountInputViewModel(ServiceType serviceType = ServiceType.SKYTEL)
        {
            _serviceType = serviceType;
            DigitCommand = new RelayCommand(OnDigit);
            BackspaceCommand = new RelayCommand(_ => OnBackspace(), _ => Input.Length > 0);
            OkCommand = new RelayCommand(async _ => await OnOkAsync(), _ => CanAccept);
            CancelCommand = new RelayCommand(_ => CancelRequested?.Invoke());
        }

        private void OnDigit(object? parameter)
        {
            if (parameter is null) return;
            if (Input.Length >= MaxLengthConst) return;
            Input += parameter.ToString();
        }

        private void OnBackspace()
        {
            if (Input.Length == 0) return;
            Input = Input[..^1];
        }

        private async Task OnOkAsync()
        {
            if (!CanAccept) return;

            try
            {
                var exists = await AccountExistsAsync(Input);
                if (!exists)
                {
                    MessageRequested?.Invoke("Анхаар", "Оруулсан дугаар буруу байна.");
                    return;
                }

                PhoneInfoRequested?.Invoke(Input, _serviceType);
            }
            catch (Exception ex)
            {
                MessageRequested?.Invoke("Алдаа", ex.Message);
            }
        }

        private async Task<bool> AccountExistsAsync(string? number)
        {
            if (string.IsNullOrWhiteSpace(number)) return false;

            string basePath = AppDomain.CurrentDomain.BaseDirectory ?? Directory.GetCurrentDirectory();
            string settingsPath = Path.Combine(basePath, "appsettings.json");
            if (!File.Exists(settingsPath))
            {
                settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "KioskApp.UI", "appsettings.json");
            }
            if (!File.Exists(settingsPath))
                throw new FileNotFoundException("appsettings.json олдсонгүй", settingsPath);

            using var fs = File.OpenRead(settingsPath);
            using var doc = await JsonDocument.ParseAsync(fs);
            if (!doc.RootElement.TryGetProperty("Database", out var db))
                throw new InvalidDataException("appsettings.json-д Database хэсэг байхгүй");

            string host = db.GetProperty("Host").GetString() ?? "localhost";
            string port = db.GetProperty("Port").GetString() ?? "5432";
            string name = db.GetProperty("Name").GetString() ?? string.Empty;
            string username = db.GetProperty("Username").GetString() ?? string.Empty;
            string password = db.TryGetProperty("Password", out var pw) ? pw.GetString() ?? string.Empty : string.Empty;

            var connStr = $"Host={host};Port={port};Username={username};Password={password};Database={name};Ssl Mode=Disable;";
            var dbService = new DatabaseService(connStr);
            await using var conn = await dbService.GetOpenConnectionAsync();

            var sql = "SELECT 1 FROM public.accounts WHERE account_number = @p AND service_type = @st LIMIT 1";
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@p", number);
            cmd.Parameters.AddWithValue("@st", (int)_serviceType);
            var res = await cmd.ExecuteScalarAsync();
            return res != null;
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
