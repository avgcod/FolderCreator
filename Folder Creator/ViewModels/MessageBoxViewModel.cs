using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Folder_Creator.Models;

namespace Folder_Creator.ViewModels
{
    public partial class MessageBoxViewModel(Window currentWindow, IMessenger theMessenger) : ViewModelBase(theMessenger), IRecipient<NotificationMessage>
    {
        /// <summary>
        /// The window this class is the data context for.
        /// </summary>
        private readonly Window _currentWindow = currentWindow;

        /// <summary>
        /// The text of the message to display.
        /// </summary>
        [ObservableProperty]
        private string _messageText = string.Empty;

        /// <summary>
        /// Handles the OK button being clicked.
        /// </summary>
        [RelayCommand]
        public void OK()
        {
            _currentWindow.Close();
            IsActive = false;
        }

        /// <summary>
        /// Handles NotificationMessages.
        /// </summary>
        /// <param name="message">The NotificationMessage received.</param>
        public void Receive(NotificationMessage message)
        {
            MessageText = message.MessageText;
        }

        /// <summary>
        /// Handles setup when the class is activated.
        /// </summary>
        protected override void OnActivated()
        {
            Messenger.RegisterAll(this);
            base.OnActivated();
        }

        /// <summary>
        /// Handles cleanup when the class is deactivated.
        /// </summary>
        protected override void OnDeactivated()
        {
            Messenger.UnregisterAll(this);
            base.OnDeactivated();
        }
    }
}
