using System.Windows;
using System.Windows.Controls;
using KioskSkytel.KioskApp.UI.Views;
using KioskSkytel.KioskApp.UI.Views.SkyMedia;
using KioskSkytel.KioskApp.UI.Views.Cally;
using KioskSkytel.KioskApp.UI.Views.SkyNet;
using KioskSkytel.KioskApp.UI.Views.GoPlus;
using System.Windows.Media;
using System.Collections.Generic;

namespace KioskSkytel
{
    public partial class MainWindow : Window
    {
        private Button? _selectedButton;
        private readonly List<Button> _menuButtons = new List<Button>();
        public MainWindow()
        {
            InitializeComponent();
            RegisterMenuButtons();
            SelectMenu(BtnSkytel);
            ShowWelcomeScreen();

        }

        private void RegisterMenuButtons()
        {
            _menuButtons.Add(BtnSkytel);
            _menuButtons.Add(BtnSkymedia);
            _menuButtons.Add(BtnGoPlus);
            _menuButtons.Add(BtnSkynet);
            _menuButtons.Add(BtnCally);
        }

        // =========================
        // DEFAULT SCREEN
        // =========================
        private void ShowWelcomeScreen()
        {
            MainContent.Content = new Skytel();
        }

        // =========================
        // MENU BUTTON EVENTS
        // =========================


        private void SelectMenu(Button button)
        {
            // Бүх товчлуурын сонголтыг цэвэрлэх
            foreach (var btn in _menuButtons)
                btn.Tag = null;

            // Сонгогдсон товчлуурт Tag өгөх
            button.Tag = "Selected";
            _selectedButton = button;
        }

        private void BtnSkytel_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
                SelectMenu(button);

            MainContent.Content = new Skytel(); // MainContent is ContentControl
        }

        private void BtnSkymedia_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
                SelectMenu(button);

            MainContent.Content = new SkyMedia();
        }

        private void BtnGoPlus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
                SelectMenu(button);

            MainContent.Content = new GoPlus();
        }

        private void BtnSkynet_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
                SelectMenu(button);

            MainContent.Content = new SkyNet();
        }

        private void BtnCally_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
                SelectMenu(button);

            MainContent.Content = new Cally();
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
