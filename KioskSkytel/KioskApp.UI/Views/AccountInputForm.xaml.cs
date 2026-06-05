using System;
using System.Windows;
using System.Windows.Input;
using KioskApp.Models;
using KioskSkytel.KioskApp.UI.ViewModels;

namespace KioskSkytel.KioskApp.UI.Views
{
    /// <summary>
    /// Interaction logic for AccountInputForm.xaml
    /// </summary>
    public partial class AccountInputForm : Window
    {
        public event Action? OnClose;
        public string? AccountNumber { get; private set; }
        private AccountInputViewModel? _viewModel;
        private readonly ServiceType _serviceType = ServiceType.SKYTEL;

        public AccountInputForm(ServiceType serviceType = ServiceType.SKYTEL)
        {
            _serviceType = serviceType;
            InitializeComponent();

            _viewModel = new AccountInputViewModel(_serviceType);
            DataContext = _viewModel;

            _viewModel.MessageRequested += (title, message) =>
            {
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            };

            _viewModel.PhoneInfoRequested += (accountNumber, serviceType) =>
            {
                AccountNumber = accountNumber;
                var window = new PhoneInfo(accountNumber, serviceType)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                Hide();
                window.ShowDialog();
                Close();
            };

            _viewModel.CancelRequested += () => { DialogResult = false; Close(); };

            Loaded += (_, __) => Keyboard.Focus(this);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs? e)
        {
            _viewModel?.CancelCommand.Execute(null);
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
                _viewModel?.DigitCommand.Execute(digit);
            }
            // Numpad numbers
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                string digit = ((int)(e.Key - Key.NumPad0)).ToString();
                _viewModel?.DigitCommand.Execute(digit);
            }
            // Backspace
            else if (e.Key == Key.Back)
            {
                _viewModel?.BackspaceCommand.Execute(null);
            }
            // Enter
            else if (e.Key == Key.Enter)
            {
                _viewModel?.OkCommand.Execute(null);
            }
            // Esc
            else if (e.Key == Key.Escape)
            {
                CancelBtn_Click(this, null);
            }
        }
    }
}
