using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using KioskApp.Models;
using KioskApp.Services;
using KioskSkytel.KioskApp.UI.Helpers;

namespace KioskSkytel.KioskApp.UI.ViewModels
{
    public class MessageRequestedEventArgs : EventArgs
    {
        public string Title { get; }
        public string Message { get; }

        public MessageRequestedEventArgs(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }

    public class BuyCardViewModel : ViewModelBase
    {
        private bool _isLoading;
        private string? _errorMessage;

        public ObservableCollection<Card> Cards { get; } = new();
        public ICommand BuyCommand { get; }
        public ICommand CloseCommand { get; }

        public event EventHandler<MessageRequestedEventArgs>? MessageRequested;
        public event EventHandler? RequestClose;

        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public BuyCardViewModel()
        {
            BuyCommand = new RelayCommand(OnBuy, CanBuy);
            CloseCommand = new RelayCommand(_ => RequestClose?.Invoke(this, EventArgs.Empty));
        }

        public async Task LoadCardsAsync(CardRepository repository)
        {
            if (repository is null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            IsLoading = true;
            ErrorMessage = null;

            try
            {
                var cards = await repository.GetAllCardsAsync();
                Cards.Clear();

                foreach (var card in cards)
                {
                    Cards.Add(card);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                MessageRequested?.Invoke(this, new MessageRequestedEventArgs("Алдаа", ex.Message));
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanBuy(object? parameter)
        {
            return parameter is Card;
        }

        private void OnBuy(object? parameter)
        {
            if (parameter is Card card)
            {
                var message = $"Сонгосон багц: {card.Price} - {card.DataGB}GB / {card.Duration}";
                MessageRequested?.Invoke(this, new MessageRequestedEventArgs("Худалдан авах", message));
            }
        }
    }
}
