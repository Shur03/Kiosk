using System.Windows;

namespace KioskSkytel.KioskApp.Views.SkyMedia
{
    public partial class AddressChange : Window
    {
        public AddressChange()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: validate and pass selected address back
            this.DialogResult = true;
            this.Close();
        }
    }
}