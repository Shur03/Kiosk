using KioskApp.Models;
using KioskSkytel.KioskApp.Views.SkyMedia;
using System.Windows;
using System.Windows.Controls;

namespace KioskSkytel.KioskApp.UI.Views.SkyMedia
{
    /// <summary>
    /// Interaction logic for SkyMedia.xaml
    /// </summary>
    public partial class SkyMedia : UserControl
    {
        private Border? _overlayBorder;
        public SkyMedia()
        {
            InitializeComponent();
        }

        private void CardGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double buttonSize = CardGrid.ActualWidth / 3;
            CardGrid.ColumnDefinitions[0].Width = new GridLength(buttonSize);
            CardGrid.ColumnDefinitions[1].Width = new GridLength(buttonSize);
            if (CardGrid.RowDefinitions.Count > 0)
            {
                CardGrid.RowDefinitions[0].Height = new GridLength(buttonSize);
            }
            //CardGrid.RowDefinitions[1].Height = new GridLength(buttonSize);
        }

        private void BtnPayment_Click(object sender, RoutedEventArgs e)
        {
            var window = new AccountInputForm(ServiceType.SKYMEDIA, AccountInputAction.BuyCard);
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }

        public void BtnGetOrder_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.ShowGetOrderWindow();
            }
        }
    }
}
