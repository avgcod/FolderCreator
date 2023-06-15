using Avalonia.Controls;
using Folder_Creator.ViewModels;
using System.Collections.Generic;

namespace Folder_Creator.Commands
{
    public class ChooseFileCommand : CommandBase
    {
        private readonly InformationViewModel _informationViewModel;
        private readonly Window _currentWindow;

        public ChooseFileCommand(InformationViewModel informationViewModel, Window currentWindow)
        {
            _informationViewModel = informationViewModel;
            _currentWindow = currentWindow;
        }

        public override async void Execute(object? parameter)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AllowMultiple = false;

            FileDialogFilter filters = new FileDialogFilter();
            filters.Name = "CSV Files (.csv)";
            filters.Extensions.Add("csv");

            ofd.Filters = new List<FileDialogFilter>();
            ofd.Filters.Add(filters);

            string[] theFile = await ofd.ShowAsync(_currentWindow) ?? new string[] { string.Empty };
            if (!string.IsNullOrWhiteSpace(theFile[0]))
            {
                _informationViewModel.SpreadsheetFile = theFile[0];
            }
        }
    }
}
