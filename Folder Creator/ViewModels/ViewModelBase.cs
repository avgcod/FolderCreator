using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Folder_Creator.ViewModels
{
    public class ViewModelBase(IMessenger theMessenger) : ObservableRecipient(theMessenger);
}
