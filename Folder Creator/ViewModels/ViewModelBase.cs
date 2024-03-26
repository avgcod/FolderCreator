using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Folder_Creator.ViewModels
{
    /// <summary>
    /// Base class for view models that inherits from ObservableRecipient.
    /// </summary>
    /// <param name="theMessenger">The IMessenger to use for the class.</param>
    public class ViewModelBase(IMessenger theMessenger) : ObservableRecipient(theMessenger);
}
