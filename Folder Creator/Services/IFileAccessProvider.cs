using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Folder_Creator.Services
{
    public interface IFileAccessProvider
    {
        public string LoadDestination(string destinationFile);
        public bool SaveDestination(string destinationFile, string destination);
        public bool CreateFolders(string spreadsheetFile, string location);
        public Task<string> LoadDestinationAsync(string destinationFile);
        public Task<bool> SaveDestinationAsync(string destinationFile, string destination);
        public Task<bool> CreateFoldersAsync(string spreadsheetFile, string location);
        public Task<IStorageFolder?> ChooseDestinationAsync();
        public Task<IStorageFile?> ChoosePackersFileAsync();
    }
}
