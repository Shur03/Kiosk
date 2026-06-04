using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KioskSkytel.KioskApp.UI.Views.Cally
{
    /// <summary>
    /// Interaction logic for Cally.xaml
    /// </summary>
    public partial class Cally : UserControl
    {
        public Cally()
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
            var window = new Payment();
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();

            //if (window.ShowDialog() == true)
            //{
            //    MessageBox.Show(
            //        $"Дугаар: {window.PhoneNumber}\nТөлбөр амжилттай хийгдлээ.",
            //        "Амжилттай",
            //        MessageBoxButton.OK,
            //        MessageBoxImage.Information);
            //}
        }
}
}
