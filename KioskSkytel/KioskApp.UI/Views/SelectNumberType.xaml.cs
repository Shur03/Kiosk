using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using KioskApp.Models;
using KioskApp.Services.Database;
using KioskApp.Services.Repository;
using System.Windows.Media;

namespace KioskSkytel.KioskApp.UI.Views
{
    public partial class SelectNumberType : UserControl
    {
        private Border _overlayBorder;

        public Window Owner { get; internal set; }

        public SelectNumberType()
        {
            InitializeComponent();
            Loaded += Skytel_Loaded;
        }

        private void Skytel_Loaded(object sender, RoutedEventArgs e)
        {
            _overlayBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0)),
                Visibility = Visibility.Collapsed,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            if (Parent is Panel parent)
            {
                parent.Children.Add(_overlayBorder);
                Panel.SetZIndex(_overlayBorder, 999);
            }
        }

        private async void BtnSelectType_Click(object sender, RoutedEventArgs e)
        {
            var numberType = (sender as Button)?.Tag as string ?? "hybrid";
            var owner = Window.GetWindow(this);
            var grid = new NumberGrid(numberType)
            {
                Owner = owner,
            };

            if (grid.ShowDialog() == true && !string.IsNullOrWhiteSpace(grid.SelectedNumber))
            {
                var scanWindow = new CardScan(grid.SelectedNumber)
                {
                    Owner = owner,
                };
                scanWindow.ShowDialog();

                if (!string.IsNullOrWhiteSpace(scanWindow.RegisterNumber))
                {
                    try
                    {
                        var dbService = CreateDatabaseService();
                        var accountRepo = new AccountRepository(dbService);

                        var firstName = string.Empty;
                        var lastName = string.Empty;
                        if (!string.IsNullOrWhiteSpace(scanWindow.FullName))
                        {
                            var nameParts = scanWindow.FullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (nameParts.Length > 0)
                            {
                                lastName = nameParts[0];
                                if (nameParts.Length > 1)
                                {
                                    firstName = string.Join(" ", nameParts, 1, nameParts.Length - 1);
                                }
                            }
                        }

                        await accountRepo.CreateAccountForRegisterNumberAsync(grid.SelectedNumber, scanWindow.RegisterNumber, ServiceType.SKYTEL, firstName, lastName);
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show($"Аккаунт үүсгэх үед алдаа гарлаа: {ex.Message}", MessageBoxType.Error);
                        return;
                    }

                    var accountInfoWindow = new AccountInfo(grid.SelectedNumber, ServiceType.SKYTEL)
                    {
                        Owner = owner,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };
                    accountInfoWindow.ShowDialog();
                }
            }
        }

        private void CloseOverlay()
        {
            if (_overlayBorder == null) return;
            _overlayBorder.Visibility = Visibility.Collapsed;
            _overlayBorder.Child = null;
        }

        private void CardGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double buttonSize = CardGrid.ActualWidth / 3;
            CardGrid.ColumnDefinitions[0].Width = new GridLength(buttonSize);
            CardGrid.ColumnDefinitions[1].Width = new GridLength(buttonSize);
            CardGrid.RowDefinitions[0].Height = new GridLength(buttonSize);
            CardGrid.RowDefinitions[1].Height = new GridLength(buttonSize);
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
