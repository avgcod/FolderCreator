using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Folder_Creator.ViewModels;
using Folder_Creator.Views;
using Folder_Creator.Services;

namespace Folder_Creator
{
    public class App : Application
    {
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
                desktop.MainWindow.DataContext = new InformationViewModel(desktop.MainWindow, _destinationFileName);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
