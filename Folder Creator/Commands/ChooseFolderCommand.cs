using Avalonia.Controls;
using Folder_Creator.ViewModels;

namespace Folder_Creator.Commands
{
    public class ChooseFolderCommand : CommandBase
    {
        private readonly InformationViewModel _informationViewModel;
        private readonly Window _currentWindow;
        public ChooseFolderCommand(InformationViewModel informationViewModel, Window currentWindow)
        {
            _informationViewModel = informationViewModel;
            _currentWindow = currentWindow;
        }

        public override void Execute(object? parameter)
        {
            OpenFolderDialog ofd = new OpenFolderDialog();

            string theDirectory = ofd.ShowAsync(_currentWindow)?.Result ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(theDirectory))
            {
                _informationViewModel.DestinationLocation = theDirectory;
            }
        }
    }
}
