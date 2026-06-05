using System.Windows;

namespace KioskSkytel.KioskApp.UI.Views.GoPlus
{
    public partial class GP_Card : Window
    {
        public string SelectedPaymentMethod { get; private set; } = string.Empty;

        // Bindable properties — set these before opening the window
        // public string Username { get; set; } = "96950039";
        // public string SelectedCard { get; set; } = "GoPlus 9900₮";
        // public string Amount { get; set; } = "9,900₮";
        // public string ReceiptType { get; set; } = "Хувь хүн";

        public GP_Card()
        {
            InitializeComponent();
        }

        private void BankCard_Click(object sender, RoutedEventArgs e)
        {
            SelectedPaymentMethod = "BankCard";
            DialogResult = true;
            Close();
        }

        private void QPay_Click(object sender, RoutedEventArgs e)
        {
            SelectedPaymentMethod = "QPay";
            DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}