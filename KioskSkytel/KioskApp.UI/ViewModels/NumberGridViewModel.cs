using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using KioskApp.Services.Api;
using KioskApp.Services.DTOs;
using KioskSkytel.KioskApp.UI.Helpers;

namespace KioskSkytel.KioskApp.UI.ViewModels
{
    public class NumberGridViewModel : ViewModelBase
    {
        private readonly NumberSearchService _searchService = new();
        private readonly string _numberType;
        private string _activeFilter = "all";
        private bool _isLoading;

        public NumberGridViewModel(string numberType)
        {
            _numberType = numberType;
            FilterCommand = new RelayCommand(async parameter => await ApplyFilterAsync(parameter?.ToString() ?? "all"));
            SelectNumberCommand = new RelayCommand(OnSelectNumber);
        }

        public ObservableCollection<NumberItem> FilteredNumbers { get; } = new();

        public string ActiveFilter
        {
            get => _activeFilter;
            private set => SetProperty(ref _activeFilter, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        public ICommand FilterCommand { get; }
        public ICommand SelectNumberCommand { get; }

        public event EventHandler<string>? NumberSelected;
        public event EventHandler<string>? LoadFailed;

        public async Task LoadAsync(string? filter = null)
        {
            await ApplyFilterAsync(filter ?? ActiveFilter);
        }

        private async Task ApplyFilterAsync(string filter)
        {
            if (IsLoading)
                return;

            ActiveFilter = filter;
            IsLoading = true;

            try
            {
                var response = await _searchService.SearchAsync(new NumberSearchRequest
                {
                    Type = _numberType,
                    SearchType = MapFilterToSearchType(filter),
                    Number = "69******",
                    Page = 1,
                    Limit = 108,
                });

                FilteredNumbers.Clear();
                foreach (var item in response.Numbers!
                             .Where(n => !string.IsNullOrWhiteSpace(n.PhoneNumber))
                             .Select(n => new NumberItem
                             {
                                 Number = n.PhoneNumber!,
                                 PhoneId = n.PhoneId ?? string.Empty,
                                 Price = n.Price,
                             }))
                {
                    FilteredNumbers.Add(item);
                }
            }
            catch (Exception ex)
            {
                LoadFailed?.Invoke(this, ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private static string MapFilterToSearchType(string filter) => filter switch
        {
            "goe" => "nice",
            "azyn" => "lucky",
            _ => "all",
        };

        private void OnSelectNumber(object? parameter)
        {
            if (parameter is not string number || string.IsNullOrWhiteSpace(number))
                return;

            NumberSelected?.Invoke(this, number);
        }
    }
}
