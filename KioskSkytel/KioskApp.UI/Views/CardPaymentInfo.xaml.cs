using System.Windows;
using KioskApp.Models;

namespace KioskSkytel.KioskApp.UI.Views
{
    /// <summary>
    /// Interaction logic for CardPaymentInfo.xaml
    /// </summary>
    public partial class CardPaymentInfo : Window
    {
        public string AccountNumber { get; }
        public string CardTitle { get; }
        public string PriceFormatted { get; }

        public CardPaymentInfo(string accountNumber, Card cardInfo)
        {
            InitializeComponent();
            AccountNumber = accountNumber;
            CardTitle = cardInfo.Title;
            PriceFormatted = $"{cardInfo.Price:N0}₮";
            DataContext = this;
        }

        public void BankCard_Click(object sender, RoutedEventArgs e)
        {
            // Handle bank card click event
        }

        public void QPay_Click(object sender, RoutedEventArgs e) { }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close(); ;
        }
    }
}
