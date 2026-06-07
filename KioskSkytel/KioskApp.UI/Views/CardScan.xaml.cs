using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using KioskApp.Models;
using KioskSkytel.KioskApp.Services.HardwareMock;

namespace KioskSkytel.KioskApp.UI.Views
{
    /// <summary>
    /// Interaction logic for CardScan.xaml
    /// </summary>
    public partial class CardScan : Window, INotifyPropertyChanged
    {
        private readonly IdCardReaderService _readerService = new();
        private string _statusText = "Иргэний үнэмлэхээ уншуулна уу";
        private string _registerNumber = string.Empty;
        private string _fullName = string.Empty;
        private string _errorText = string.Empty;
        private bool _isCancelled = false;

        public string? SelectedNumber { get; }

        public string StatusText { get => _statusText; set => SetProperty(ref _statusText, value); }
        public string RegisterNumber { get => _registerNumber; set => SetProperty(ref _registerNumber, value); }
        public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
        public string ErrorText { get => _errorText; set => SetProperty(ref _errorText, value); }

        public CardScan(string? selectedNumber = null)
        {
            SelectedNumber = selectedNumber;
            InitializeComponent();
            DataContext = this;
            Loaded += CardScan_Loaded;
        }

        private async void CardScan_Loaded(object? sender, RoutedEventArgs e)
        {
            await ScanCardAsync();
        }

        private async Task ScanCardAsync()
        {
            StatusText = "Иргэний үнэмлэхээ уншуулна уу";
            ErrorText = string.Empty;
            RegisterNumber = string.Empty;
            FullName = string.Empty;
            HideInfoBorder();

            try
            {
                // Stage 1: Wait for card to be present
                bool cardDetected = false;
                while (!_isCancelled && !cardDetected)
                {
                    cardDetected = await Task.Run(() => _readerService.IsCardPresent());
                    if (!cardDetected)
                    {
                        StatusText = "Иргэний үнэмлэхээ уншуулна уу";
                        ErrorText = string.Empty;
                        HideInfoBorder();
                        await Task.Delay(500);
                    }
                }

                if (_isCancelled)
                    return;

                // Stage 2: Card is present, read it
                StatusText = "Карт уншиж байна...";
                await Task.Delay(300);

                var info = await Task.Run(() => _readerService.ReadCard());

                if (info.Success)
                {
                    RegisterNumber = info.RegisterNumber ?? string.Empty;
                    FullName = string.Join(' ', new[] { info.LastName, info.FirstName }).Trim();
                    StatusText = "Карт амжилттай уншигдлаа";
                    ErrorText = string.Empty;
                    ShowInfoBorder();
                    DialogResult = true;
                    return;
                }

                // Treat certain messages as fatal (reader missing or PC/SC system errors)
                var err = info.ErrorMessage ?? string.Empty;
                if (err.Contains("OMNIKEY reader not found", StringComparison.OrdinalIgnoreCase)
                    || err.Contains("PC/SC error", StringComparison.OrdinalIgnoreCase)
                    || err.Contains("Connect failed", StringComparison.OrdinalIgnoreCase))
                {
                    StatusText = "Алдаа гарлаа";
                    ErrorText = err;
                    ShowErrorText();
                    HideInfoBorder();
                    return;
                }

                // Non-fatal read error: go back to waiting for card
                StatusText = "Иргэний үнэмлэхээ уншуулна уу";
                ErrorText = string.Empty;
                HideInfoBorder();
                await ScanCardAsync(); // Retry
            }
            catch (Exception ex)
            {
                StatusText = "Алдаа гарлаа";
                ErrorText = ex.Message;
                ShowErrorText();
                HideInfoBorder();
            }
        }

        private void ShowInfoBorder()
        {
            if (FindName("IdCardBorder") is FrameworkElement idCardBorder)
                idCardBorder.Visibility = Visibility.Collapsed;
            if (FindName("InfoBorder") is FrameworkElement infoBorder)
                infoBorder.Visibility = Visibility.Visible;
        }

        private void HideInfoBorder()
        {
            if (FindName("IdCardBorder") is FrameworkElement idCardBorder)
                idCardBorder.Visibility = Visibility.Visible;
            if (FindName("InfoBorder") is FrameworkElement infoBorder)
                infoBorder.Visibility = Visibility.Collapsed;
        }

        private void ShowErrorText()
        {
            if (FindName("ErrorTextBlock") is FrameworkElement errorText)
                errorText.Visibility = Visibility.Visible;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            _isCancelled = true;
            Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(field, value)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
