using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Avalonia.Platform.Storage;
using Avalonia.Controls;
using CsvHelper;
using System.Globalization;
using Folder_Creator.Models;

namespace Folder_Creator.Services
{
    public class FileAccessService : IFileAccessProvider
    {
        private readonly Window _currentWindow;

        public FileAccessService(Window currentWindow)
        {
            _currentWindow = currentWindow;
        }

        public string LoadDestination(string destinationFile)
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
        public bool SaveDestination(string destinationFile, string destination)
        {
            try
            {
                using (TextWriter writer = new StreamWriter(File.OpenWrite(destinationFile)))
                {
                    writer.WriteLine(destination);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public bool CreateFolders(string spreadsheetFile, string location)
        {
            if (File.Exists(spreadsheetFile) && Directory.Exists(location))
            {
                try
                {
                    using StreamReader thesReader = new StreamReader(spreadsheetFile);
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                return false;
            }

        }

        public async Task<string> LoadDestinationAsync(string destinationFile)
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
        public async Task<bool> SaveDestinationAsync(string destinationFile, string destination)
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
                throw new Exception(ex.Message);
            }

        }
        public async Task<bool> CreateFoldersAsync(string spreadsheetFile, string location)
        {
            if (File.Exists(spreadsheetFile) && Directory.Exists(location))
            {
                try
                {
                    using StreamReader thesReader = new StreamReader(spreadsheetFile);
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                return false;
            }

        }
        public async Task<IStorageFolder?> ChooseDestinationAsync()
        {
            var folders = await _currentWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                Title = "Select Destination Folder",
                AllowMultiple = false
            });

            return folders.Count >= 1 ? folders[0] : null;
        }
        public async Task<IStorageFile?> ChoosePackersFileAsync()
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

            IReadOnlyList<IStorageFile> files = await _currentWindow?.StorageProvider.OpenFilePickerAsync(options);

            return files.Count >= 1 ? files[0] : null;
        }

    }
}
