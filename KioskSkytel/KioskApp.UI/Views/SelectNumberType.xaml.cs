using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KioskSkytel.KioskApp.UI.Views
{
    public partial class SelectNumberType : UserControl
    {
        private Border _overlayBorder;

        public Window Owner { get; internal set; }

        public SelectNumberType()
        {
            InitializeComponent();
            Loaded += Skytel_Loaded;
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

            if (Parent is Panel parent)
            {
                parent.Children.Add(_overlayBorder);
                Panel.SetZIndex(_overlayBorder, 999);
            }
        }

        private void BtnSelectType_Click(object sender, RoutedEventArgs e)
        {
            var numberType = (sender as Button)?.Tag as string ?? "hybrid";
            var owner = Window.GetWindow(this);
            var grid = new NumberGrid(numberType)
            {
                Owner = owner,
            };

            if (grid.ShowDialog() == true && !string.IsNullOrWhiteSpace(grid.SelectedNumber))
            {
                var scanWindow = new CardScan(grid.SelectedNumber)
                {
                    Owner = owner,
                };
                scanWindow.ShowDialog();
            }
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
