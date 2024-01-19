using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Avalonia.Platform.Storage;
using Avalonia.Controls;
using CsvHelper;
using System.Globalization;
using Folder_Creator.Models;
using CommunityToolkit.Mvvm.Messaging;

namespace Folder_Creator.Services
{
    /// <summary>
    /// This class provide access to the local file system.
    /// </summary>
    public static class FileAccessService
    {
        /// <summary>
        /// Loads the saved destination folder.
        /// </summary>
        /// <param name="destinationFile">The file to load from.</param>
        /// <param name="theMessenger">The messenger to use in case of error.</param>
        /// <returns>The destination folder or an empty string if there was an error.</returns>
        public static async Task<string> LoadDestinationAsync(string destinationFile, IMessenger theMessenger)
        {
            try
            {
                string destination = string.Empty;
                if (File.Exists(destinationFile))
                {
                    using TextReader reader = new StreamReader(File.OpenRead(destinationFile));
                    destination = await reader.ReadLineAsync() ?? string.Empty;
                }
                return destination;
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
                return string.Empty;
            }
        }

        /// <summary>
        /// Saves the destination folder to a file.
        /// </summary>
        /// <param name="destinationFile">The file to save to.</param>
        /// <param name="destination">The destination folder to save.</param>
        /// <param name="theMessenger">The messenger to use in case of error.</param>
        public static async Task<bool> SaveDestinationAsync(string destinationFile, string destination, IMessenger theMessenger)
        {
            try
            {
                await using TextWriter writer = new StreamWriter(File.OpenWrite(destinationFile));
                await writer.WriteLineAsync(destination);
                return true;
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Creates the folders for each line of a CSV file.
        /// </summary>
        /// <param name="csvFile">The CSV file.</param>
        /// <param name="location">The location to create the folders in.</param>
        /// <param name="theMessenger">The messenger to use in case of error.</param>
        /// <returns>if the operation succeeded.</returns>
        public static async Task<bool> CreateFoldersAsync(string csvFile, string location, IMessenger theMessenger)
        {
            try
            {
                List<Task> createDirectoryTasks = [];
                if (File.Exists(csvFile) && Directory.Exists(location))
                {
                    using StreamReader thesReader = new(csvFile);
                    using CsvReader thecReader = new(thesReader, CultureInfo.InvariantCulture);

                    await foreach (Packer currentPacker in thecReader.GetRecordsAsync<Packer>())
                    {
                        if (!Directory.Exists(Path.Combine(location, currentPacker.PackerNumber)))
                        {
                            createDirectoryTasks.Add(Task.Run(() => Directory.CreateDirectory(location + Path.DirectorySeparatorChar + currentPacker.PackerNumber)));
                        }
                    }

                    await Task.WhenAll(createDirectoryTasks);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Opens a folder chooser dialog for the user to choose a folder.
        /// </summary>
        /// <param name="_currentWindow">The current window of the application.</param>
        /// <param name="theMessenger">The messenger to use in case of error.</param>
        /// <returns>The selected folder or null if there was an error.</returns>
        public static async Task<string> ChooseDestinationAsync(Window _currentWindow, IMessenger theMessenger)
        {
            string folderName = string.Empty;
            try
            {
                FolderPickerOpenOptions options = new()
                {
                    Title = "Select Destination Folder",
                    AllowMultiple = false
                };

                if (_currentWindow?.StorageProvider is { CanOpen: true } storageProvider)
                {
                    IReadOnlyList<IStorageFolder> folders = await storageProvider.OpenFolderPickerAsync(options);

                    if (folders.Count > 0 && folders[0].CanBookmark)
                    {
                        folderName = await folders[0].SaveBookmarkAsync() ?? string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

                return folderName;
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
                return folderName;
            }
        }

        /// <summary>
        /// Opens a file chooser dialog for the user to choose a file.
        /// </summary>
        /// <param name="_currentWindow">The current window of the application.</param>
        /// <param name="theMessenger">The messenger to use in case of error.</param>
        /// <returns>The selected file or null if there was an error.</returns>
        public static async Task<string> ChooseCSVFileAsync(Window _currentWindow, IMessenger theMessenger)
        {
            FilePickerFileType fileTypes = new("CSV Files (.csv)")
            {
                Patterns = new[] { "*.csv" },
                AppleUniformTypeIdentifiers = new[] { "public.csv" },
                MimeTypes = new[] { "csv/*" }
            };

            FilePickerOpenOptions options = new()
            {
                Title = "Choose csv file.",
                AllowMultiple = false,
                FileTypeFilter = new FilePickerFileType[] { fileTypes }
            };

            string fileName = string.Empty;

            try
            {
                if (_currentWindow?.StorageProvider is { CanOpen: true } storageProvider)
                {
                    IReadOnlyList<IStorageFile> files = await storageProvider.OpenFilePickerAsync(options);
                    if (files.Count > 0 && files[0].CanBookmark)
                    {
                        fileName = await files[0].SaveBookmarkAsync() ?? string.Empty;
                    }
                }

                return fileName;
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
                return fileName;
            }
        }
    }
}
