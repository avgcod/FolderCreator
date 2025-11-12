using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Folder_Creator.ViewModels;

public partial class MessageBoxViewModel(Window currentWindow, string messageText) : ObservableObject {

    [ObservableProperty] private string _messageText = messageText;

    [RelayCommand] public void OK() => currentWindow.Close();
}
