using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Folder_Creator.ViewModels;
using Folder_Creator.Views;
using Microsoft.Extensions.DependencyInjection;

using static Folder_Creator.Helpers.ServiceCollectionExtensions;

namespace Folder_Creator {
    public class App : Application {

        public override void Initialize() {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted() {
            ServiceProvider provider = CreateServiceCollectionwithCommonServices().BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);

                desktop.MainWindow = provider.GetRequiredService<InformationView>();
                //desktop.MainWindow.Closing += MainWindow_Closing;
                //ivModel = new MainWindowViewModel(desktop.MainWindow, _destinationFileName, StrongReferenceMessenger.Default);
                desktop.MainWindow.DataContext = provider.GetRequiredService<InformationViewModel>();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
