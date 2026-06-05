using System.Windows;
using KioskApp.Models;

namespace KioskSkytel.KioskApp.UI.Views
{
    /// <summary>
    /// Interaction logic for PhoneInfo.xaml
    /// </summary>
    public partial class PhoneInfo : Window
    {
        private KioskSkytel.KioskApp.UI.ViewModels.PhoneInfoViewModel? _viewModel;

        public PhoneInfo(string accountNumber, ServiceType serviceType = ServiceType.SKYTEL)
        {
            InitializeComponent();
            _viewModel = new KioskSkytel.KioskApp.UI.ViewModels.PhoneInfoViewModel(accountNumber, serviceType);
            DataContext = _viewModel;
            Loaded += async (_, __) => await _viewModel.LoadAsync();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
