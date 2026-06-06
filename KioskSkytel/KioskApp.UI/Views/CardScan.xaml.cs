using System.Windows;

namespace KioskSkytel.KioskApp.UI.Views 
{
    /// <summary>
    /// Interaction logic for CardScan.xaml
    /// </summary>
    public partial class CardScan : Window
    {
        public string? SelectedNumber { get; }

        public CardScan(string? selectedNumber = null)
        {
            SelectedNumber = selectedNumber;
            InitializeComponent();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
