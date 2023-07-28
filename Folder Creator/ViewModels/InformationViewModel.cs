using Avalonia.Controls;
using System.ComponentModel;
using System;
using System.Threading.Tasks;
using Folder_Creator.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Folder_Creator.Views;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Platform.Storage;

namespace Folder_Creator.ViewModels
{
    public partial class InformationViewModel : ViewModelBase
    {
        #region Variables
        private readonly string _destinationFile;
        private readonly Window _currentWindow;
        #endregion

        #region Properties
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
        private string spreadsheetFile = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
        private string destinationLocation = string.Empty;

        [ObservableProperty]
        private bool _busy = false;

        [ObservableProperty]
        private string _creatingText = "Create";

        public bool CanCreate => !string.IsNullOrEmpty(SpreadsheetFile) &&
                !string.IsNullOrEmpty(DestinationLocation) &&
                !Busy;
        public bool CanChoose => !Busy;
        #endregion

        public InformationViewModel(Window currentWindow, string destinationFile)
        {
            _destinationFile = destinationFile;
            _currentWindow = currentWindow;

            _currentWindow.Opened += OnWindowOpened;
            _currentWindow.Closing += OnWindowClosing;
        }

        public async Task LoadDestinationAsync()
        {
            DestinationLocation = await FileAccessService.LoadDestinationAsync(_destinationFile);
        }

        [RelayCommand(CanExecute = nameof(CanCreate))]
        public async Task Create()
        {
            Busy = true;
            CreatingText = "Creating...";

            //_fileAccessService.CreateFolders(SpreadsheetFile, DestinationLocation);
            await FileAccessService.CreateFoldersAsync(SpreadsheetFile, DestinationLocation);

            CreatingText = "Create";

            MessageBoxView mboxView = new MessageBoxView();
            mboxView.DataContext = new MessageBoxViewModel(mboxView, "Finished Creating");
            await mboxView.ShowDialog(_currentWindow);

            Busy = false;
        }

        [RelayCommand(CanExecute = nameof(CanChoose))]
        public async Task ChooseFolder()
        {
            IStorageFolder? selectedFolder = await FileAccessService.ChooseDestinationAsync(_currentWindow);

            if (selectedFolder != null && selectedFolder.CanBookmark)
            {
                DestinationLocation = await selectedFolder?.SaveBookmarkAsync();
            }
        }

        [RelayCommand(CanExecute = nameof(CanChoose))]
        public async Task ChooseFile()
        {
            IStorageFile? selectedFile = await FileAccessService.ChoosePackersFileAsync(_currentWindow);

            if (selectedFile != null && selectedFile.CanBookmark)
            {
                SpreadsheetFile = await selectedFile?.SaveBookmarkAsync();
            }
        }

        public async void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            _currentWindow.Opened -= OnWindowOpened;
            _currentWindow.Closing -= OnWindowClosing;

            await FileAccessService.SaveDestinationAsync(_destinationFile, DestinationLocation);
        }

        private async void OnWindowOpened(object? sender, EventArgs e)
        {
            await LoadDestinationAsync();
        }

    }
}
