using Avalonia.Controls;
using Folder_Creator.Commands;
using System.ComponentModel;
using System.Windows.Input;
using System;
using System.Threading.Tasks;
using Folder_Creator.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Folder_Creator.Views;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Platform.Storage;
using System.ComponentModel.DataAnnotations;

namespace Folder_Creator.ViewModels
{
    public partial class InformationViewModel : ObservableValidator
    {
        #region Variables
        private readonly FileAccessService _fileAccessService;
        private readonly string _destinationFile;
        private readonly Window _currentWindow;
        #endregion

        #region Properties
        [ObservableProperty]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Spreadsheet file is required")]
        [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
        private string spreadsheetFile = string.Empty;

        [ObservableProperty]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Destination location is required")]
        [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
        private string destinationLocation = string.Empty;

        [ObservableProperty]
        [IsFalse]
        private bool _busy = false;

        [ObservableProperty]
        private string _creatingText = "Create";

        public bool CanCreate => !string.IsNullOrEmpty(SpreadsheetFile) &&
                !string.IsNullOrEmpty(DestinationLocation) &&
                !Busy;
        #endregion

        public InformationViewModel(Window currentWindow, string destinationFile)
        {
            _destinationFile = destinationFile;
            _currentWindow = currentWindow;
            _fileAccessService = new FileAccessService(_currentWindow);

            _currentWindow.Opened += OnWindowOpened;
            _currentWindow.Closing += OnWindowClosing;
        }

        public async Task LoadDestinationAsync()
        {
            DestinationLocation = await _fileAccessService.LoadDestinationAsync(_destinationFile);
        }              

        [RelayCommand(CanExecute = nameof(CanCreate))]
        public async Task Create()
        {
            Busy = true;
            CreatingText = "Creating...";

            //_fileAccessService.CreateFolders(SpreadsheetFile, DestinationLocation);
            await _fileAccessService.CreateFoldersAsync(SpreadsheetFile, DestinationLocation);

            CreatingText = "Create";

            MessageBoxView mboxView = new MessageBoxView();
            mboxView.DataContext = new MessageBoxViewModel(mboxView, "Finished Creating");
            await mboxView.ShowDialog(_currentWindow);

            Busy = false;
        }

        [RelayCommand]
        public async Task ChooseFolder()
        {
            ValidateProperty(false, nameof(Busy));

            if (!HasErrors)
            {
                IStorageFolder? selectedFolder = await _fileAccessService.ChooseDestinationAsync();

                if (selectedFolder != null)
                {
                    if (selectedFolder.CanBookmark)
                    {
                        DestinationLocation = await selectedFolder.SaveBookmarkAsync();
                    }
                }
            }

        }

        [RelayCommand]
        public async Task ChooseFile()
        {
            ValidateProperty(false, nameof(Busy));

            if (!HasErrors)
            {
                IStorageFile? selectedFile = await _fileAccessService.ChoosePackersFileAsync();

                if (selectedFile != null)
                {
                    if (selectedFile.CanBookmark)
                    {
                        SpreadsheetFile = await selectedFile.SaveBookmarkAsync();
                    }
                }
            }

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

    public sealed class IsFalseAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            return bool.TryParse(value?.ToString(), out bool result) && result == false
                ? ValidationResult.Success
                : new ValidationResult("Value must be false");
        }

    }

    public sealed class IsTrueAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            return bool.TryParse(value?.ToString(), out bool result) && result == true
                ? ValidationResult.Success
                : new ValidationResult("Value must be true");
        }

    }
}
