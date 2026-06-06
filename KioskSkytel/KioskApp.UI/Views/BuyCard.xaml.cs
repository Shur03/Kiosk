using KioskApp.Models;
using KioskSkytel.KioskApp.UI.ViewModels;
using System.Windows;
using static KioskApp.Models.Card;

namespace KioskSkytel.KioskApp.UI.Views
{
    public partial class BuyCard : Window
    {
        private readonly BuyCardViewModel _viewModel;
        private readonly string _accountNumber;

        public Card? SelectedCard => _viewModel.SelectedCard?.Card;

        public BuyCard(CardCategory cardType = CardCategory.SKYTEL, string accountNumber = "")
        {
            InitializeComponent();
            _viewModel = new BuyCardViewModel(cardType);
            _accountNumber = accountNumber;
            _viewModel.ContinueRequested += (_, _) =>
            {
                DialogResult = true;
                Close();
            };
            DataContext = _viewModel;
            Loaded += BuyCard_Loaded;
        }

        private async void BuyCard_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await _viewModel.LoadAsync();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Карт ачаалах", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BuyCardClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedCard?.Card is not Card selectedCard)
            {
                MessageBox.Show("Эхлээд карт сонгоно уу.", "Анхаар", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var window = new CardPaymentInfo(_accountNumber, selectedCard)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.ShowDialog();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
