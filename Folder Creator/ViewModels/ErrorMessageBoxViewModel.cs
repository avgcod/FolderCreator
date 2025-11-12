using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Folder_Creator.ViewModels;

public partial class ErrorMessageBoxViewModel(Window currentWindow, string errorType, string errorMessage) : ObservableObject {

    [ObservableProperty] private string _errorType = errorType;

    [ObservableProperty] private string _errorMessage = errorMessage;

    [RelayCommand] public void OK() => currentWindow.Close();
}
