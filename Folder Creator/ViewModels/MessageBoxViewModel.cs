using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Folder_Creator.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Folder_Creator.ViewModels
{
    public partial class MessageBoxViewModel : ObservableObject
    {
        private readonly Window _currentWindow;

        [ObservableProperty]
        private string _messageText = string.Empty;


        public MessageBoxViewModel(Window currentWindow, string messageText)
        {
            MessageText = messageText;
            _currentWindow = currentWindow;

            _currentWindow.Opened += OnWindowOpened;
            _currentWindow.Closing += OnWindowClosing;
        }

        [RelayCommand]
        public void OK()
        {
            _currentWindow.Close();
        }

        private void OnWindowClosing(object? sender, CancelEventArgs e)
        {
            _currentWindow.Opened -= OnWindowOpened;
            _currentWindow.Closing -= OnWindowClosing;
        }

        private void OnWindowOpened(object? sender, EventArgs e)
        {
            _currentWindow.FindControl<Button>("btnOk")?.Focus();
        }
    }
}
