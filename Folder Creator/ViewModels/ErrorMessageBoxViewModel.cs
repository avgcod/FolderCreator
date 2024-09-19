using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace Folder_Creator.ViewModels
{
    public partial class ErrorMessageBoxViewModel(Window theWindow,IMessenger theMessenger, string errorMessage, string errorType) : ViewModelBase(theMessenger)
    {
        /// <summary>
        /// The window this class is the data context for.
        /// </summary>
        private readonly Window _currentWindow = theWindow;

        /// <summary>
        /// The string name of the error type.
        /// </summary>
        [ObservableProperty]
        private string _errorType = errorMessage;

        /// <summary>
        /// The error message.
        /// </summary>
        [ObservableProperty]
        private string _errorMessage = errorType;

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
