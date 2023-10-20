using Avalonia.Controls;
using System.ComponentModel;
using System;
using System.Threading.Tasks;
using Folder_Creator.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Folder_Creator.Views;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Messaging;
using Folder_Creator.Models;

namespace Folder_Creator.ViewModels
{
    public partial class InformationViewModel : ViewModelBase, IRecipient<OperationErrorMessage>
    {
        #region Variables
        /// <summary>
        /// The file to save the destination to.
        /// </summary>
        private readonly string _destinationFile;
        /// <summary>
        /// The program window.
        /// </summary>
        private readonly Window _currentWindow;
        /// <summary>
        /// The messenger to use.
        /// </summary>
        private readonly IMessenger _theMessenger;
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
        /// Tetx for the Create button.
        /// </summary>
        [ObservableProperty]
        private string _creatingText = "Create";        
        #endregion

        public InformationViewModel(Window currentWindow, string destinationFile, IMessenger theMessenger)
        {
            _destinationFile = destinationFile;
            _currentWindow = currentWindow;
            _theMessenger = theMessenger;

            _theMessenger.Register<OperationErrorMessage>(this);
            _currentWindow.Opened += OnWindowOpened;
            _currentWindow.Closing += OnWindowClosing;
        }

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

        public async Task LoadDestinationAsync()
        {
            DestinationLocation = await FileAccessService.LoadDestinationAsync(_destinationFile, _theMessenger);
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

            await FileAccessService.CreateFoldersAsync(CsvFile, DestinationLocation, _theMessenger);

            CreatingText = "Create";

            MessageBoxView mboxView = new MessageBoxView();
            mboxView.DataContext = new MessageBoxViewModel(mboxView, "Finished Creating");
            await mboxView.ShowDialog(_currentWindow);

            Busy = false;
        }

        /// <summary>
        /// Opens a folder chooser when the Choose Destination button is clicked and sets the DestinationLocation property to the selected folder.
        /// </summary>
        /// <returns>Task</returns>
        [RelayCommand(CanExecute = nameof(CanChoose))]
        public async Task ChooseFolder()
        {
            IStorageFolder? selectedFolder = await FileAccessService.ChooseDestinationAsync(_currentWindow,_theMessenger);

            if (selectedFolder != null && selectedFolder.CanBookmark)
            {
                DestinationLocation = await selectedFolder?.SaveBookmarkAsync() ?? string.Empty;
            }
        }

        /// <summary>
        /// Opens a file chooser when the Choose Spreadsheet button is clicked and sets the SpreadsheetFile property to the selected file.
        /// </summary>
        /// <returns>Task</returns>
        [RelayCommand(CanExecute = nameof(CanChoose))]
        public async Task ChooseFile()
        {
            IStorageFile? selectedFile = await FileAccessService.ChooseCSVFileAsync(_currentWindow, _theMessenger);

            if (selectedFile != null && selectedFile.CanBookmark)
            {
                CsvFile = await selectedFile?.SaveBookmarkAsync() ?? string.Empty;
            }
        } 
        #endregion

        public async void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            _theMessenger.UnregisterAll(this);
            _currentWindow.Opened -= OnWindowOpened;
            _currentWindow.Closing -= OnWindowClosing;

            await FileAccessService.SaveDestinationAsync(_destinationFile, DestinationLocation, _theMessenger);
        }

        private async void OnWindowOpened(object? sender, EventArgs e)
        {
            await LoadDestinationAsync();
        }

        /// <summary>
        /// Handles OperationErrorMessage messages.
        /// </summary>
        /// <param name="message">The OperationErrorMessage to handle.</param>
        /// <returns>Task</returns>
        private async Task HandleOperationErrorMessageAsync(OperationErrorMessage message)
        {
            ErrorMessageBoxView emboxView = new ErrorMessageBoxView();

            emboxView.DataContext = new ErrorMessageBoxViewModel(emboxView, message);

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
