using System.Windows;
using KioskApp.Models;

namespace KioskSkytel.KioskApp.UI.Views
{
    /// <summary>
    /// Interaction logic for PhoneInfo.xaml
    /// </summary>
    public partial class AccountInfo : Window
    {
        private KioskSkytel.KioskApp.UI.ViewModels.AccountInfoViewModel? _viewModel;

        public AccountInfo(string accountNumber, ServiceType serviceType = ServiceType.SKYTEL)
        {
            InitializeComponent();
            _viewModel = new KioskSkytel.KioskApp.UI.ViewModels.AccountInfoViewModel(accountNumber, serviceType);
            DataContext = _viewModel;
            Loaded += async (_, __) => await _viewModel.LoadAsync();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ContinueBtnClick(object sender, RoutedEventArgs e)
        {
            var window = new AccountPaymentInfo();
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
