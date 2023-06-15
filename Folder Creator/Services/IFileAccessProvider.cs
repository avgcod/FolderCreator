using System.Collections.Generic;
using System.Threading.Tasks;

namespace Folder_Creator.Services
{
    public interface IFileAccessProvider
    {
        string GetDestination(string destinationFile);
        bool SaveDestination(string destinationFile, string destination);
        bool CreateFolders(string spreadsheetFile, string location);
        Task<string> GetDestinationAsync(string destinationFile);
        Task<bool> SaveDestinationAsync(string destinationFile, string destination);
        Task<bool> CreateFoldersAsync(string spreadsheetFile, string location);
    }
}
