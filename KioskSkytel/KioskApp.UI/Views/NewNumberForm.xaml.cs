using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KioskSkytel.KioskApp.UI.Views
{
    public partial class NewNumberForm : UserControl
    {
        // Event for when form is closed/completed
        public event EventHandler<FormResultEventArgs> FormCompleted;
        public event EventHandler FormCancelled;

        public NewNumberForm()
        {
            InitializeComponent();
            Loaded += NewNumberForm_Loaded;
        }

        private void NewNumberForm_Loaded(object sender, RoutedEventArgs e)
        {
            PhoneNumberCombo.Focus();
        }

        private void ServiceType_Checked(object sender, RoutedEventArgs e)
        {
            if (PrepaidRadio.IsChecked == true)
            {
                UpdatePhoneNumbers(true);
            }
            else if (PostpaidRadio.IsChecked == true)
            {
                UpdatePhoneNumbers(false);
            }
        }

        private void UpdatePhoneNumbers(bool isPrepaid)
        {
            PhoneNumberCombo.Items.Clear();
            if (isPrepaid)
            {
                PhoneNumberCombo.Items.Add(new ComboBoxItem { Content = "8811 0001" });
                PhoneNumberCombo.Items.Add(new ComboBoxItem { Content = "8811 0002" });
                PhoneNumberCombo.Items.Add(new ComboBoxItem { Content = "8811 0003" });
                PhoneNumberCombo.Items.Add(new ComboBoxItem { Content = "8811 0005" });
                PhoneNumberCombo.Items.Add(new ComboBoxItem { Content = "8811 0008" });
            }
            else
            {
                PhoneNumberCombo.Items.Add(new ComboBoxItem { Content = "9911 0001" });
                PhoneNumberCombo.Items.Add(new ComboBoxItem { Content = "9911 0002" });
                PhoneNumberCombo.Items.Add(new ComboBoxItem { Content = "9911 0003" });
                PhoneNumberCombo.Items.Add(new ComboBoxItem { Content = "9911 0005" });
                PhoneNumberCombo.Items.Add(new ComboBoxItem { Content = "9911 0008" });
            }
            PhoneNumberCombo.SelectedIndex = 0;
        }

        private void PhoneNumberCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomNumberCheck.IsChecked == true)
            {
                CustomNumberCheck.IsChecked = false;
                CustomNumberBox.IsEnabled = false;
                CustomNumberBox.Clear();
            }
        }

        private void CustomNumberCheck_Checked(object sender, RoutedEventArgs e)
        {
            CustomNumberBox.IsEnabled = true;
            CustomNumberBox.Focus();
            PhoneNumberCombo.IsEnabled = false;
        }

        private void CustomNumberCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            CustomNumberBox.IsEnabled = false;
            CustomNumberBox.Clear();
            PhoneNumberCombo.IsEnabled = true;
            NumberErrorText.Visibility = Visibility.Collapsed;
        }

        private void CustomNumberBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string input = CustomNumberBox.Text;

            if (!string.IsNullOrEmpty(input) && !Regex.IsMatch(input, @"^\d*$"))
            {
                CustomNumberBox.Text = Regex.Replace(input, @"[^\d]", "");
                CustomNumberBox.CaretIndex = CustomNumberBox.Text.Length;
            }

            if (!string.IsNullOrEmpty(CustomNumberBox.Text))
            {
                if (CustomNumberBox.Text.Length != 8)
                {
                    NumberErrorText.Text = "Дугаар 8 оронтой байх ёстой";
                    NumberErrorText.Visibility = Visibility.Visible;
                }
                else
                {
                    NumberErrorText.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                NumberErrorText.Visibility = Visibility.Collapsed;
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;

            var result = new FormResult
            {
                PhoneNumber = GetSelectedPhoneNumber(),
                ServiceType = PrepaidRadio.IsChecked == true ? "Урьдчилсан төлбөрт" : "Дараа төлбөрт",
                UserName = string.IsNullOrEmpty(UserNameBox.Text) ? "Тодорхойгүй" : UserNameBox.Text,
                Email = string.IsNullOrEmpty(EmailBox.Text) ? "Тодорхойгүй" : EmailBox.Text
            };

            StatusMessage.Text = "✅ Амжилттай захиалагдлаа!";
            StatusMessage.Foreground = Brushes.Green;

            // Trigger event
            FormCompleted?.Invoke(this, new FormResultEventArgs(result));

            // Auto hide after 2 seconds
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                ClearForm();
                StatusMessage.Text = "";
            };
            timer.Start();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(UserNameBox.Text))
            {
                ShowError("Хэрэглэгчийн нэрээ оруулна уу!");
                UserNameBox.Focus();
                UserNameBox.BorderBrush = Brushes.Red;
                return false;
            }
            else
            {
                UserNameBox.BorderBrush = (Brush)FindResource("PhoneBorderBrush");
            }

            if (!string.IsNullOrWhiteSpace(EmailBox.Text))
            {
                if (!IsValidEmail(EmailBox.Text))
                {
                    ShowError("Имэйл хаяг буруу байна!");
                    EmailBox.Focus();
                    EmailBox.BorderBrush = Brushes.Red;
                    return false;
                }
                else
                {
                    EmailBox.BorderBrush = (Brush)FindResource("PhoneBorderBrush");
                }
            }

            if (CustomNumberCheck.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(CustomNumberBox.Text))
                {
                    ShowError("Утасны дугаараа оруулна уу!");
                    CustomNumberBox.Focus();
                    return false;
                }

                if (CustomNumberBox.Text.Length != 8)
                {
                    ShowError("Утасны дугаар 8 оронтой байх ёстой!");
                    CustomNumberBox.Focus();
                    return false;
                }
            }

            return true;
        }

        private string GetSelectedPhoneNumber()
        {
            if (CustomNumberCheck.IsChecked == true)
            {
                return CustomNumberBox.Text;
            }
            else
            {
                var selectedItem = PhoneNumberCombo.SelectedItem as ComboBoxItem;
                return selectedItem?.Content.ToString() ?? "Сонгогдоогүй";
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ShowError(string message)
        {
            StatusMessage.Text = $"⚠️ {message}";
            StatusMessage.Foreground = Brushes.Red;

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                StatusMessage.Text = "";
            };
            timer.Start();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            FormCancelled?.Invoke(this, EventArgs.Empty);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            FormCancelled?.Invoke(this, EventArgs.Empty);
        }

        private void ClearForm()
        {
            PrepaidRadio.IsChecked = true;
            UserNameBox.Clear();
            EmailBox.Clear();
            CustomNumberCheck.IsChecked = false;
            PhoneNumberCombo.IsEnabled = true;
            PhoneNumberCombo.SelectedIndex = 0;
            StatusMessage.Text = "";
            UserNameBox.BorderBrush = (Brush)FindResource("PhoneBorderBrush");
            EmailBox.BorderBrush = (Brush)FindResource("PhoneBorderBrush");
        }
    }

    // Result classes
    public class FormResult
    {
        public string PhoneNumber { get; set; }
        public string ServiceType { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class FormResultEventArgs : EventArgs
    {
        public FormResult Result { get; set; }

        public FormResultEventArgs(FormResult result)
        {
            Result = result;
        }
    }
}