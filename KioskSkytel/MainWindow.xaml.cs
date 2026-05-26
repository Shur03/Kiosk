using System.Windows;
using System.Windows.Controls;

namespace KioskSkytel
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Default screen
            ShowWelcomeScreen();
        }

        // =========================
        // DEFAULT SCREEN
        // =========================
        private void ShowWelcomeScreen()
        {
            MainContent.Content = CreatePlaceholderView(
                "WELCOME",
                ""
            );
        }

        // =========================
        // MENU BUTTON EVENTS
        // =========================

        private void BtnSkytel_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = CreatePlaceholderView(
                "SKYTEL",
                ""
            );
        }

        private void BtnSkymedia_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = CreatePlaceholderView(
                "SKYMEDIA",
                ""
            );
        }

        private void BtnGoPlus_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = CreatePlaceholderView(
                "GO+",
                ""
            );
        }

        private void BtnSkynet_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = CreatePlaceholderView(
                "SKYNET",
                ""
            );
        }

        private void BtnCally_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = CreatePlaceholderView(
                "CALLY",
                ""
            );
        }

        // =========================
        // TEMP VIEW GENERATOR
        // =========================

        private UIElement CreatePlaceholderView(string title, string subtitle)
        {
            var stack = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var titleBlock = new TextBlock
            {
                Text = title,
                FontSize = 52,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var subtitleBlock = new TextBlock
            {
                Text = subtitle,
                FontSize = 20,
                Foreground = System.Windows.Media.Brushes.White,
                Opacity = 0.8,
                Margin = new Thickness(0, 12, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            stack.Children.Add(titleBlock);
            stack.Children.Add(subtitleBlock);

            return stack;
        }
    }
}