using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Folder_Creator.Helpers;
using Folder_Creator.Services;
using Folder_Creator.Views;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Folder_Creator.ViewModels;

public partial class InformationViewModel : ViewModelBase {
    private readonly string _destinationFile;
    private readonly InformationView _currentWindow;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private string csvFile = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private string destinationLocation = string.Empty;

    [ObservableProperty] private bool _busy = false;

    [ObservableProperty] private string _creatingText = "Create";

    public InformationViewModel(InformationView currentWindow, IConfigurationRoot config) {
        _currentWindow = currentWindow;
        _currentWindow.Opened += CurrentWindow_Opened;
        _currentWindow.Closed += CurrentWindow_Closed;

        _destinationFile = config.GetRequiredSection("MyConfigOptions").GetSection("DestinationFileLocation").Value!;
    }

    private async void CurrentWindow_Closed(object? sender, EventArgs e) => (await FileAccessService.SaveDestination(_destinationFile, DestinationLocation))
        .IfFail(async error => await ShowErrorMessageBox(error.GetType().ToString(), error.Message));

    private async void CurrentWindow_Opened(object? sender, EventArgs e) => (await LoadDestination())
        .Match(location => DestinationLocation = location, async theError => await ShowErrorMessageBox("Error", theError.Message));

    public bool CanCreate => !CsvFile.IsEmpty() && !DestinationLocation.IsEmpty() && !Busy;

    public bool CanChoose => !Busy;

    public async Task<Fin<string>> LoadDestination() {
        return await FileAccessService.LoadDestination(_destinationFile);
    }

    [RelayCommand(CanExecute = nameof(CanCreate))]
    public async Task Create() {
        Busy = true;
        CreatingText = "Creating...";

        Fin<string> possibleResults = await FileAccessService.CreateFolders(CsvFile, DestinationLocation);

        await possibleResults.Match(async successResult => await ShowMessageBox(successResult), async errorResult => await ShowErrorMessageBox(errorResult.GetType().ToString(), errorResult.Message));

        CreatingText = "Create";
        Busy = false;
    }

    private async Task ShowMessageBox(string message) {
        MessageBoxView mboxView = new();
        MessageBoxViewModel mbvModel = new(mboxView, message);
        mboxView.DataContext = mbvModel;
        mboxView.SizeToContent = SizeToContent.WidthAndHeight;
        await mboxView.ShowDialog(_currentWindow);
    }
    private async Task ShowErrorMessageBox(string errorType, string errorMessage) {
        ErrorMessageBoxView emboxView = new();
        ErrorMessageBoxViewModel embvModel = new(emboxView, errorType, errorMessage);
        emboxView.DataContext = embvModel;
        emboxView.SizeToContent = SizeToContent.WidthAndHeight;
        await emboxView.ShowDialog(_currentWindow);
    }

    [RelayCommand(CanExecute = nameof(CanChoose))]
    public async Task ChooseFolder() => (await FileAccessService.ChooseDestination(_currentWindow)).IfSome(pathString => DestinationLocation = pathString);

    [RelayCommand(CanExecute = nameof(CanChoose))]
    public async Task ChooseFile() => (await FileAccessService.ChooseCSVFile(_currentWindow)).IfSome(fileString => CsvFile = fileString);
}
