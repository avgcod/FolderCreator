using Avalonia.Controls;
using Folder_Creator.Services;
using Folder_Creator.ViewModels;
using Folder_Creator.Views;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Folder_Creator.Commands
{
    public class CreateCommand : CommandBase
    {
        private readonly InformationViewModel _informationViewModel;
        private readonly FileAccessService _faService;
        private readonly Window _currentWindow;
        public CreateCommand(Window currentWindow, InformationViewModel informationViewModel, FileAccessService faService)
        {
            _currentWindow = currentWindow;
            _informationViewModel = informationViewModel;
            _faService = faService;

            _informationViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
        public override bool CanExecute(object? parameter)
        {
            return !string.IsNullOrEmpty(_informationViewModel.SpreadsheetFile) &&
                !string.IsNullOrEmpty(_informationViewModel.DestinationLocation) &&
                !_informationViewModel.Busy &&
                base.CanExecute(parameter);
        }
        public async override void Execute(object? parameter)
        {
            _informationViewModel.Busy = true;
            _informationViewModel.CreatingText = "Creating...";

            //_faService.CreateFolders(_informationViewModel.SpreadsheetFile, _informationViewModel.DestinationLocation);
            await _faService.CreateFoldersAsync(_informationViewModel.SpreadsheetFile, _informationViewModel.DestinationLocation);

            _informationViewModel.CreatingText = "Create";

            MessageBoxView mboxView = new MessageBoxView();
            mboxView.DataContext = new MessageBoxViewModel(mboxView, "Finished Creating");
            await mboxView.ShowDialog(_currentWindow);

            _informationViewModel.Busy = false;

        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(InformationViewModel.SpreadsheetFile) ||
                e.PropertyName == nameof(InformationViewModel.DestinationLocation) ||
                e.PropertyName == nameof(InformationViewModel.Busy))
            {
                OnCanExecutedChanged();
            }
        }
    }
}
