using Avalonia.Controls;
using Folder_Creator.Commands;
using System.ComponentModel;
using System.Windows.Input;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using Folder_Creator.Services;

namespace Folder_Creator.ViewModels
{
    public class InformationViewModel : ReactiveObject
    {
        #region Variables
        private readonly FileAccessService _fileAccessService;
        private readonly string _destinationFile;
        private readonly Window _currentWindow;
        #endregion

        #region Properties
        private string spreadsheetFile = string.Empty;
        public string SpreadsheetFile
        {
            get
            {
                return spreadsheetFile;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref spreadsheetFile, value);
            }
        }

        private string destinationLocation = string.Empty;
        public string DestinationLocation
        {
            get
            {
                return destinationLocation;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref destinationLocation, value);
            }

        }

        private bool _busy = false;
        public bool Busy
        {
            get
            {
                return _busy;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _busy, value);
            }
        }

        private string _creatingText = "Create";
        public string CreatingText
        {
            get
            {
                return _creatingText;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _creatingText, value);
            }
        }
        #endregion

        #region Commands
        public ICommand ChooseFileCommand { get; }
        public ICommand ChooseFolderCommand { get; }
        public ICommand CreateCommand { get; }
        #endregion

        public InformationViewModel(Window currentWindow, FileAccessService fileAccessServivce, string destinationFile)
        {
            _fileAccessService = fileAccessServivce;
            _destinationFile = destinationFile;
            _currentWindow = currentWindow;

            _currentWindow.Opened += OnWindowOpened;
            _currentWindow.Closing += OnWindowClosing;

            ChooseFileCommand = new ChooseFileCommand(this, _currentWindow);
            ChooseFolderCommand = new ChooseFolderCommand(this, _currentWindow);
            CreateCommand = new CreateCommand(currentWindow, this, _fileAccessService);
        }

        public async Task LoadDestinationAsync()
        {
            DestinationLocation = await _fileAccessService.GetDestinationAsync(_destinationFile);
        }

        public async void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            _currentWindow.Opened -= OnWindowOpened;
            _currentWindow.Closing -= OnWindowClosing;

            await _fileAccessService.SaveDestinationAsync(_destinationFile, DestinationLocation);
        }

        private async void OnWindowOpened(object? sender, EventArgs e)
        {
            await LoadDestinationAsync();
        }

    }
}
