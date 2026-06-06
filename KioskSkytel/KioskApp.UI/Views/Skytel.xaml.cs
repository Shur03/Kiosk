using KioskApp.Models;
using KioskSkytel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static KioskApp.Models.Card;

namespace KioskSkytel.KioskApp.UI.Views
{
    public partial class Skytel : UserControl
    {
        private Border? _overlayBorder;
        private readonly CardCategory _cardType = CardCategory.SKYTEL;

        public Skytel()
        {
            InitializeComponent();
            this.Loaded += Skytel_Loaded;
        }

        private void Skytel_Loaded(object sender, RoutedEventArgs e)
        {
            _overlayBorder = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0)),
                Visibility = Visibility.Collapsed,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            if (this.Parent is Panel parent)
            {
                parent.Children.Add(_overlayBorder);
                Panel.SetZIndex(_overlayBorder, 999);
            }
        }

        private void BtnNewNumber_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.ShowSelectSimType();
            }
        }

        private void BtnPayment_Click(object sender, RoutedEventArgs e)
        {
            var window = new AccountInputForm(ServiceType.SKYTEL);
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

        private void BtnBuyCard_Click(object sender, RoutedEventArgs e)
        {
            var window = new AccountInputForm(ServiceType.SKYTEL, AccountInputAction.BuyCard);
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }

        private void BtnSimRecovery_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void CloseOverlay()
        {
            if (_overlayBorder == null) return;
            _overlayBorder.Visibility = Visibility.Collapsed;
            _overlayBorder.Child = null;
        }

        private void CardGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double buttonSize = CardGrid.ActualWidth / 3;
            CardGrid.ColumnDefinitions[0].Width = new GridLength(buttonSize);
            CardGrid.ColumnDefinitions[1].Width = new GridLength(buttonSize);
            CardGrid.RowDefinitions[0].Height = new GridLength(buttonSize);
            CardGrid.RowDefinitions[1].Height = new GridLength(buttonSize);
        }
    }
}