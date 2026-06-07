using System.Windows;
using System.Windows.Controls;
using KioskSkytel.KioskApp.Views.SkyMedia;

namespace KioskSkytel.KioskApp.UI.Views.SkyMedia
{
    /// <summary>
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : UserControl
    {
        private Border? _overlayBorder;
        public OrderWindow()
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

        private void BtnBuyCard_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddressChange();
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }

        public void BtnChangeAddress_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddressChange();
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }
    }
}
