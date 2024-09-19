using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using Folder_Creator.ViewModels;
using Folder_Creator.Views;

namespace Folder_Creator
{
    public class App : Application
    {
        private InformationViewModel? ivModel;
        private const string _destinationFileName = "destination.txt";

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new InformationView();
                desktop.MainWindow.Closing += MainWindow_Closing;
                ivModel = new InformationViewModel(desktop.MainWindow, _destinationFileName, StrongReferenceMessenger.Default);
                desktop.MainWindow.DataContext = ivModel;
                ivModel.IsActive = true;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void MainWindow_Closing(object? sender, Avalonia.Controls.WindowClosingEventArgs e)
        {
            ivModel!.IsActive = false;
        }
    }
}
