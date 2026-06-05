using System.Windows;
using System.Windows.Controls;
using KioskSkytel.KioskApp.UI.Views.GoPlus;

namespace KioskSkytel.KioskApp.Views
{
    public partial class GP_Login : Window
    {
        public string EnteredText => InputTextBox.Text;

        public GP_Login()
        {
            InitializeComponent();
        }

        private void Key_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
                InputTextBox.Text += btn.Content?.ToString();
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (InputTextBox.Text.Length > 0)
                InputTextBox.Text = InputTextBox.Text[..^1];
        }

        private void MicButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: integrate speech recognition
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {

            var window = new GP_Card();
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }
    }
}