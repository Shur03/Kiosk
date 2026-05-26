using System.Windows;
using System.Windows.Controls;
using KioskSkytel.KioskApp.UI.Views;
using System.Windows.Media;

namespace KioskSkytel
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowWelcomeScreen();
        }

        // =========================
        // DEFAULT SCREEN
        // =========================
        private void ShowWelcomeScreen()
        {
            MainContent.Content = CreatePlaceholderView("WELCOME", "Зүүн цэснээс үйлчилгээ сонгоно уу");
        }

        // =========================
        // MENU BUTTON EVENTS
        // =========================

        private void BtnSkytel_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Skytel(); // MainContent is ContentControl
        }

        private void BtnSkymedia_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = CreatePlaceholderView("SKYMEDIA", "Медиа үйлчилгээ");
        }

        private void BtnGoPlus_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = CreatePlaceholderView("GO+", "GO+ үйлчилгээ");
        }

        private void BtnSkynet_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = CreatePlaceholderView("SKYNET", "Интернет үйлчилгээ");
        }

        private void BtnCally_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = CreatePlaceholderView("CALLY", "Дуудлагын үйлчилгээ");
        }

        // =========================
        // PLACEHOLDER VIEW
        // =========================
        private UIElement CreatePlaceholderView(string title, string subtitle)
        {
            var stack = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            stack.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 52,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            stack.Children.Add(new TextBlock
            {
                Text = subtitle,
                FontSize = 20,
                Foreground = Brushes.White,
                Opacity = 0.8,
                Margin = new Thickness(0, 12, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            return stack;
        }
    }
}
