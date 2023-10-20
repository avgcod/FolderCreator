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
        public static string LoadDestination(string destinationFile, IMessenger theMessenger)
        {
            try
            {
                if (File.Exists(destinationFile))
                {
                    using (TextReader reader = new StreamReader(File.OpenRead(destinationFile)))
                    {
                        return reader.ReadLine() ?? string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
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
        public static void SaveDestination(string destinationFile, string destination, IMessenger theMessenger)
        {
            try
            {
                using (TextWriter writer = new StreamWriter(File.OpenWrite(destinationFile)))
                {
                    writer.WriteLine(destination);
                }
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
            }

        }
        /// <summary>
        /// Creates the folders for each line of a CSV file.
        /// </summary>
        /// <param name="csvFile">The CSV file.</param>
        /// <param name="location">The location to create the folders in.</param>
        /// <param name="theMessenger">The messenger to use in case of error.</param>
        /// <returns>if the operation succeeded.</returns>
        public static bool CreateFolders(string csvFile, string location, IMessenger theMessenger)
        {
            try
            {
                if (File.Exists(csvFile) && Directory.Exists(location))
                {
                    using StreamReader thesReader = new StreamReader(csvFile);
                    using CsvReader thecReader = new CsvReader(thesReader, CultureInfo.InvariantCulture);

                    IEnumerable<Packer> packers = thecReader.GetRecords<Packer>();

                    foreach (var currentPacker in packers)
                    {
                        if (!Directory.Exists(Path.Combine(location, currentPacker.PackerNumber)))
                        {
                            Directory.CreateDirectory(location + Path.DirectorySeparatorChar + currentPacker.PackerNumber);
                        }
                    }

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
                    using (TextReader reader = new StreamReader(File.OpenRead(destinationFile)))
                    {
                        destination = await reader.ReadLineAsync() ?? string.Empty;
                    }
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
                using (TextWriter writer = new StreamWriter(File.OpenWrite(destinationFile)))
                {
                    await writer.WriteLineAsync(destination);
                }
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
                if (File.Exists(csvFile) && Directory.Exists(location))
                {
                    using StreamReader thesReader = new StreamReader(csvFile);
                    using CsvReader thecReader = new CsvReader(thesReader, CultureInfo.InvariantCulture);

                    IAsyncEnumerable<Packer> packers = thecReader.GetRecordsAsync<Packer>();
                    await foreach (Packer currentPacker in packers)
                    {
                        if (!Directory.Exists(Path.Combine(location, currentPacker.PackerNumber)))
                        {
                            await Task.Run(() => Directory.CreateDirectory(location + Path.DirectorySeparatorChar + currentPacker.PackerNumber));
                        }
                    }

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
        public static async Task<IStorageFolder?> ChooseDestinationAsync(Window _currentWindow, IMessenger theMessenger)
        {
            try
            {
                var folders = await _currentWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
                {
                    Title = "Select Destination Folder",
                    AllowMultiple = false
                });

                return folders.Count >= 1 ? folders[0] : null;
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
                return null;
            }

        }
        /// <summary>
        /// Opens a file chooser dialog for the user to choose a file.
        /// </summary>
        /// <param name="_currentWindow">The current window of the application.</param>
        /// <param name="theMessenger">The messenger to use in case of error.</param>
        /// <returns>The selected file or null if there was an error.</returns>
        public static async Task<IStorageFile?> ChooseCSVFileAsync(Window _currentWindow, IMessenger theMessenger)
        {
            FilePickerFileType fileTypes = new FilePickerFileType("CSV Files (.csv)")
            {
                Patterns = new[] { "*.csv" },
                AppleUniformTypeIdentifiers = new[] { "public.csv" },
                MimeTypes = new[] { "csv/*" }
            };

            FilePickerOpenOptions options = new FilePickerOpenOptions()
            {
                Title = "Choose csv file.",
                AllowMultiple = false,
                FileTypeFilter = new FilePickerFileType[] { fileTypes }
            };

            try
            {
                IReadOnlyList<IStorageFile>? files = await _currentWindow?.StorageProvider.OpenFilePickerAsync(options);

                return files.Count >= 1 ? files[0] : null;
            }
            catch (Exception ex)
            {
                theMessenger.Send<OperationErrorMessage>(new OperationErrorMessage(ex.GetType().Name, ex.Message));
                return null;
            }
        }

    }
}
