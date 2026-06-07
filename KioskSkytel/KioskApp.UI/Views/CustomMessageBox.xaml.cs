using System.Windows;
using System.Windows.Media;

namespace KioskSkytel.KioskApp.UI.Views
{
    public enum MessageBoxType
    {
        Info,
        Success,
        Warning,
        Error,
        YesNo
    }

    public enum MessageBoxResult
    {
        Ok,
        Yes,
        No
    }

    public partial class CustomMessageBox : Window
    {
        public MessageBoxResult Result { get; private set; } = MessageBoxResult.Ok;

        // ── Constructor ──────────────────────────────────────────────
        public CustomMessageBox(string message, MessageBoxType type = MessageBoxType.Info)
        {
            InitializeComponent();

            MessageText.Text = message;
            ApplyType(type);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Ok;
            Close();
        }

        // Apply basic theming / button visibility based on type
        private void ApplyType(MessageBoxType type)
        {
            switch (type)
            {
                case MessageBoxType.Info:
                    IconText.Text = "\u2139"; // info
                    IconBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3F2FD"));
                    break;
                case MessageBoxType.Success:
                    IconText.Text = "\u2714"; // check
                    IconBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8F5E9"));
                    break;
                case MessageBoxType.Warning:
                    IconText.Text = "\u26A0"; // warning
                    IconBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF8E1"));
                    break;
                case MessageBoxType.Error:
                    IconText.Text = "\u2716"; // cross
                    IconBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEBEE"));
                    break;
                case MessageBoxType.YesNo:
                    IconText.Text = "\u2753"; // question
                    IconBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3F2FD"));
                    break;
            }
        }

        // ── Static Show helpers ──────────────────────────────────────

        /// <summary>Shows an info/success/warning/error dialog. Returns Ok always.</summary>
        public static MessageBoxResult Show(string message,
                                            MessageBoxType type = MessageBoxType.Info)
        {
            var dlg = new CustomMessageBox(message, type);
            dlg.ShowDialog();
            return dlg.Result;
        }

        /// <summary>Shows a Yes/No confirmation dialog.</summary>
        public static bool Confirm(string message)
        {
            var dlg = new CustomMessageBox(message, MessageBoxType.YesNo);
            dlg.ShowDialog();
            return dlg.Result == MessageBoxResult.Yes;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            Close();
        }

        // ── Theming per type ─────────────────────────────────────────
    }
}