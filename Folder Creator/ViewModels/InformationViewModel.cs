using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Folder_Creator.Models;
using Folder_Creator.Services;
using Folder_Creator.Views;
using System.Threading.Tasks;

namespace Folder_Creator.ViewModels
{
    /// <summary>
    /// Data context class for the InformationView Window.
    /// </summary>
    /// <param name="currentWindow">The InformationView window.</param>
    /// <param name="destinationFile">The file to save the destination location to.</param>
    /// <param name="theMessenger">the IMessenger to use.</param>
    public partial class InformationViewModel(Window currentWindow, string destinationFile, IMessenger theMessenger) : ViewModelBase(theMessenger), IRecipient<OperationErrorMessage>
    {
        #region Variables
        /// <summary>
        /// The file to save the destination to.
        /// </summary>
        private readonly string _destinationFile = destinationFile;
        /// <summary>
        /// The InformationView window.
        /// </summary>
        private readonly Window _currentWindow = currentWindow;
        #endregion

        #region Properties
        /// <summary>
        /// The csv file to use.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
        private string csvFile = string.Empty;

        /// <summary>
        /// The destination folder to user.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
        private string destinationLocation = string.Empty;

        /// <summary>
        /// If the program is currently busy.
        /// </summary>
        [ObservableProperty]
        private bool _busy = false;

        /// <summary>
        /// Text for the Create button.
        /// </summary>
        [ObservableProperty]
        private string _creatingText = "Create";

        #endregion

        #region Commands
        /// <summary>
        /// If the Create button can be clicked.
        /// </summary>
        public bool CanCreate => !string.IsNullOrEmpty(CsvFile) &&
                !string.IsNullOrEmpty(DestinationLocation) &&
                !Busy;

        /// <summary>
        /// If the Choose CSV and Choose Destination buttons can be clicked.
        /// </summary>
        public bool CanChoose => !Busy;

        /// <summary>
        /// Gets the saved previous destination or an empty string if there is no saved destination.
        /// </summary>
        /// <returns>Task</returns>
        public async Task LoadDestinationAsync()
        {
            DestinationLocation = await FileAccessService.LoadDestinationAsync(_destinationFile, Messenger);
        }

        /// <summary>
        /// Creates the folders from the SpreadsheetFile property in the DestinationLocation when the Create button is clicked.
        /// </summary>
        /// <returns>Task</returns>
        [RelayCommand(CanExecute = nameof(CanCreate))]
        public async Task Create()
        {
            Busy = true;
            CreatingText = "Creating...";

            await FileAccessService.CreateFoldersAsync(CsvFile, DestinationLocation, Messenger);

            await ShowMessageBox("Finished Creating Folders");

            CreatingText = "Create";
            Busy = false;
        }

        /// <summary>
        /// Shows a message box using the provided message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Task</returns>
        private async Task ShowMessageBox(string message)
        {
            MessageBoxView mboxView = new();
            MessageBoxViewModel mbvModel = new (mboxView, Messenger);
            mboxView.DataContext = mbvModel;
            mbvModel.IsActive = true;
            Messenger.Send(new NotificationMessage(message));
            mboxView.SizeToContent = SizeToContent.WidthAndHeight;
            await mboxView.ShowDialog(_currentWindow);
        }

        /// <summary>
        /// Opens a folder chooser when the Choose Destination button is clicked and sets the DestinationLocation property to the selected folder.
        /// </summary>
        /// <returns>Task</returns>
        [RelayCommand(CanExecute = nameof(CanChoose))]
        public async Task ChooseFolder()
        {
            DestinationLocation = await FileAccessService.ChooseDestinationAsync(_currentWindow, Messenger);
        }

        /// <summary>
        /// Opens a file chooser when the Choose Spreadsheet button is clicked and sets the SpreadsheetFile property to the selected file.
        /// </summary>
        /// <returns>Task</returns>
        [RelayCommand(CanExecute = nameof(CanChoose))]
        public async Task ChooseFile()
        {
            CsvFile = await FileAccessService.ChooseCSVFileAsync(_currentWindow, Messenger);
        }
        #endregion

        /// <summary>
        /// Handles setup when the class is acticated.
        /// </summary>
        protected override async void OnActivated()
        {
            Messenger.RegisterAll(this);
            await LoadDestinationAsync();
            base.OnActivated();
        }

        /// <summary>
        /// Handles cleanup when the class is deactivated.
        /// </summary>
        protected override async void OnDeactivated()
        {
            Messenger.UnregisterAll(this);

            await FileAccessService.SaveDestinationAsync(_destinationFile, DestinationLocation, Messenger);

            base.OnDeactivated();
        }

        /// <summary>
        /// Handles OperationErrorMessage messages.
        /// </summary>
        /// <param name="message">The OperationErrorMessage to handle.</param>
        /// <returns>Task</returns>
        private async Task HandleOperationErrorMessageAsync(OperationErrorMessage message)
        {
            ErrorMessageBoxView emboxView = new();
            ErrorMessageBoxViewModel embvModel = new (emboxView, Messenger, message.ErrorMessage, message.ErrorType);
            emboxView.DataContext = embvModel;
            embvModel.IsActive = true;
            emboxView.SizeToContent = SizeToContent.WidthAndHeight;
            await emboxView.ShowDialog(_currentWindow);
        }

        /// <summary>
        /// Received OperationErrorMessage messages.
        /// </summary>
        /// <param name="message">The OperationErrorMessage message that was received.</param>
        public async void Receive(OperationErrorMessage message)
        {
            await HandleOperationErrorMessageAsync(message);
        }
    }
}
