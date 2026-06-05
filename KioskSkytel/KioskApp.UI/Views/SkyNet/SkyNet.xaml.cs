using System.Windows;
using System.Windows.Controls;
using KioskApp.Models;
using KioskSkytel.KioskApp.UI.Views;

namespace KioskSkytel.KioskApp.UI.Views.SkyNet
{
    /// <summary>
    /// Interaction logic for SkyNet.xaml
    /// </summary>
    public partial class SkyNet : UserControl
    {
        private Border _overlayBorder;
        public SkyNet()
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
            var window = new AccountInputForm(ServiceType.SKYNET);
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }
    }
}
