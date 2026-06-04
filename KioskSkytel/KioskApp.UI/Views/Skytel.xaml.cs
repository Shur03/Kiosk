using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KioskSkytel.KioskApp.UI.Views
{
    public partial class Skytel : UserControl
    {
        private Border _overlayBorder;

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
            if (_overlayBorder == null) return;

            var form = new NewNumberForm();
            form.HorizontalAlignment = HorizontalAlignment.Center;
            form.VerticalAlignment = VerticalAlignment.Center;
            form.FormCompleted += (s, args) =>
            {
                MessageBox.Show(
                    $"Шинэ дугаар: {args.Result.PhoneNumber}\nХэрэглэгч: {args.Result.UserName}",
                    "Амжилттай",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                CloseOverlay();
            };
            form.FormCancelled += (s, args) => CloseOverlay();

            _overlayBorder.Child = form;
            _overlayBorder.Visibility = Visibility.Visible;
        }

        private void BtnPayment_Click(object sender, RoutedEventArgs e)
        {
            var window = new PaymentNew();
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
            var window = new BuyCard();
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