using CsvHelper.Configuration.Attributes;

namespace Folder_Creator.Models
{
    /// <summary>
    /// Class to hold the number to use as the folder name.
    /// </summary>
    public class FolderNumberHolder
    {
        [Name("Base Document Reference")]
        public string FolderNumber { get; set; } = string.Empty;
    }
}
