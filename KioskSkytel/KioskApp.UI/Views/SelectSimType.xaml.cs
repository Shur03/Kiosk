using System.Windows;
using System.Windows.Controls;

namespace KioskSkytel.KioskApp.UI.Views
{
    /// <summary>
    /// Interaction logic for SelectSimType.xaml
    /// </summary>
    public partial class SelectSimType : UserControl
    {
        private Border _overlayBorder;

        public Window Owner { get; internal set; }

        public SelectSimType()
        {
            InitializeComponent();
        }

        private void BtnSelectType_Click(object sender, RoutedEventArgs e)
        {
            //var simType = (sender as Button)?.Tag as string ?? "hybrid";
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.ShowSelectNumberType();
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
