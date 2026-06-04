using KioskSkytel.KioskApp.Views;
using System.Windows;
using System.Windows.Controls;

namespace KioskSkytel.KioskApp.UI.Views.GoPlus
{
    /// <summary>
    /// Interaction logic for GoPlus.xaml
    /// </summary>
    public partial class GoPlus : UserControl
    {
        private Border _overlayBorder;
        public GoPlus()
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
            var window = new GP_Login();
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }
    }
}
