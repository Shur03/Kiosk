using System.Windows;

namespace KioskSkytel
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //InitializeComponent();

            // optional: disable Alt+F4 (basic kiosk protection)
            this.Closing += (s, e) =>
            {
                e.Cancel = true;
            };
        }

        private void Services_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Services page coming soon");
        }

        private void Payments_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Payments page coming soon");
        }

        private void Balance_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Balance check coming soon");
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Support page coming soon");
        }
    }
}