using System;
using System.Windows;
using KioskSkytel.KioskApp.UI.ViewModels;

namespace KioskSkytel.KioskApp.UI.Views
{
    public partial class NumberGrid : Window
    {
        private readonly NumberGridViewModel _viewModel;

        public string? SelectedNumber { get; private set; }

        public NumberGrid(string numberType)
        {
            InitializeComponent();
            _viewModel = new NumberGridViewModel(numberType);
            _viewModel.NumberSelected += ViewModel_NumberSelected;
            _viewModel.LoadFailed += ViewModel_LoadFailed;
            DataContext = _viewModel;
            Loaded += NumberGrid_Loaded;
        }

        private async void NumberGrid_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadAsync();
        }

        private void ViewModel_NumberSelected(object? sender, string number)
        {
            SelectedNumber = number;
            DialogResult = true;
            Close();
        }

        private void ViewModel_LoadFailed(object? sender, string message)
        {
            CustomMessageBox.Show(message, MessageBoxType.Warning);
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
