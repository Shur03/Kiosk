using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using KioskApp.Models;
using KioskApp.Services.Database;
using KioskApp.Services.Repository;

namespace KioskSkytel.KioskApp.UI.Views
{
    public partial class AccountPaymentInfo : Window, INotifyPropertyChanged
    {
        public string AccountNumber { get; }
        public string CardTitle { get; }
        public string PriceFormatted { get; }
        public List<PaymentTransaction> RecentPayments { get; private set; } = new();
        public bool HasRecentPayments => RecentPayments.Count > 0;
        private readonly decimal _amount;

        public event PropertyChangedEventHandler? PropertyChanged;

        public AccountPaymentInfo()
            : this(string.Empty, new Card
            {
                Id = 0,
                Title = string.Empty,
                Price = 0,
                Duration = string.Empty,
                UnitAmount = 0,
                DataGB = 0,
                CardType = 0
            })
        {
        }

        public AccountPaymentInfo(string accountNumber, Card cardInfo)
        {
            InitializeComponent();
            AccountNumber = accountNumber;
            CardTitle = cardInfo.Title;
            _amount = Convert.ToDecimal(cardInfo.Price);
            PriceFormatted = $"{_amount:N0}₮";
            DataContext = this;
            Loaded += async (_, __) => await LoadPaymentHistoryAsync();
        }

        private async Task LoadPaymentHistoryAsync()
        {
            try
            {
                var dbService = CreateDatabaseService();
                var paymentRepository = new PaymentRepository(dbService);
                RecentPayments = await paymentRepository.GetPaymentTransactionsByAccountNumberAndStatusAsync(AccountNumber, (int)ServiceType.SKYTEL, 2);
                OnPropertyChanged(nameof(RecentPayments));
                OnPropertyChanged(nameof(HasRecentPayments));
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Төлбөрийн мэдээлэл авах үед алдаа гарлаа: {ex.Message}", MessageBoxType.Error);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void BankCard_Click(object sender, RoutedEventArgs e)
        {
            await ProcessPaymentAsync(PaymentMethodType.CREDIT);
        }

        public async void QPay_Click(object sender, RoutedEventArgs e)
        {
            await ProcessPaymentAsync(PaymentMethodType.QPAY);
        }

        private async Task ProcessPaymentAsync(PaymentMethodType paymentMethod)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AccountNumber))
                {
                    CustomMessageBox.Show("Бүртгэлийн дугаар олдсонгүй.", MessageBoxType.Error);
                    return;
                }

                var dbService = CreateDatabaseService();
                var paymentRepository = new PaymentRepository(dbService);
                var accountId = await paymentRepository.GetAccountIdByNumberAsync(AccountNumber, (int)ServiceType.SKYTEL);
                if (accountId == null)
                {
                    CustomMessageBox.Show("Зөв дугаар оруулсан эсэхээ шалгана уу.", MessageBoxType.Error);
                    return;
                }

                var invoiceId = await paymentRepository.CreateInvoiceAsync(
                    accountId.Value,
                    _amount,
                    DateTime.UtcNow,
                    1,
                    DateTime.UtcNow.AddDays(30),
                    DateTime.UtcNow);

                var transactionReference = PaymentRepository.GenerateTransactionReference(paymentMethod);
                await paymentRepository.CreatePaymentTransactionAsync(
                    invoiceId,
                    accountId.Value,
                    _amount,
                    paymentMethod,
                    transactionReference,
                    1,
                    DateTime.UtcNow);

                CustomMessageBox.Show($"Төлбөр амжилттай хийгдлээ. Баримтын дугаар: {transactionReference}", MessageBoxType.Info);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Төлбөр хийх үед алдаа гарлаа: {ex.Message}", MessageBoxType.Error);
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
