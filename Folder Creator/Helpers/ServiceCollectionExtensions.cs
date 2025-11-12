using CommunityToolkit.Mvvm.Messaging;
using Folder_Creator.ViewModels;
using Folder_Creator.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Folder_Creator.Helpers;

public static class ServiceCollectionExtensions
{
    public static ServiceCollection CreateServiceCollectionwithCommonServices()
    {
        ServiceCollection services = new();

        services.AddSingleton<InformationView>();
        services.AddSingleton<InformationViewModel>();

        services.AddScoped<MessageBoxView>();
        services.AddScoped<MessageBoxViewModel>();

        services.AddScoped<ErrorMessageBoxView>();
        services.AddScoped<ErrorMessageBoxViewModel>();

        services.AddSingleton<StrongReferenceMessenger>();
        services.AddSingleton<IMessenger, StrongReferenceMessenger>();

        services.AddSingleton<IConfigurationRoot>(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

        return services;
    }
}
