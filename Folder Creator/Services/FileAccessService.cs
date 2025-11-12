using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CsvHelper;
using Folder_Creator.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Folder_Creator.Services;

public static class FileAccessService
{
    public static async Task<Fin<string>> LoadDestination(string destinationFile) {
        return await ValidatedFilePath(destinationFile)
            .Some(async validatedPath => {
                try {
                    using TextReader reader = new StreamReader(File.OpenRead(validatedPath));
                    return await reader.ReadLineAsync() ?? string.Empty;
                } catch (Exception ex) {
                    return Fin.Fail<string>(ex);
                }
            })
            .None(() => Task.FromResult(Fin.Fail<string>(Error.New("Unable to find the provided file path."))));
    }

    private static Option<string> ValidatedFilePath(string path) {
        return File.Exists(path) switch {
            true => path,
            false => Option<string>.None
        };
    }

    public static async Task<Fin<bool>> SaveDestination(string destinationFile, string destination) {
        try {
            await using TextWriter writer = new StreamWriter(File.OpenWrite(destinationFile));
            await writer.WriteLineAsync(destination);
            return true;
        } catch (Exception ex) {
            return Fin.Fail<bool>(ex);
        }
    }

    public static async Task<Fin<string>> CreateFolders(string csvFile, string location) {
        if (!await FileExists(csvFile))
            return Fin.Fail<string>($"Unable to find file {csvFile}.");

        if (!await DirectoryExists(location))
            return Fin.Fail<string>($"Unable to find directory {location}.");

        var compiledList = await BuildDirectoryList(csvFile, location);

        Fin<string> result = Fin.Fail<string>("Not initialized");

        compiledList.Match(async directoryTaskList => result = BuildMessage(await Task.WhenAll(directoryTaskList)), theError => result = Fail(theError));

        return result;
    }

    public static async Task<Option<string>> ChooseDestination(Window _currentWindow) {
        FolderPickerOpenOptions options = new() {
            Title = "Select Destination Folder",
            AllowMultiple = false
        };

        IReadOnlyList<IStorageFolder> folders = await _currentWindow.StorageProvider.OpenFolderPickerAsync(options);

        if (folders.Any() && folders[0].CanBookmark)
            return (folders[0].TryGetLocalPath() is string localPath) ? localPath : Option<string>.None;

        return Option<string>.None;
    }

    public static async Task<Option<string>> ChooseCSVFile(Window _currentWindow) {
        FilePickerFileType fileTypes = new("CSV Files (.csv)") {
            Patterns = ["*.csv"],
            AppleUniformTypeIdentifiers = ["public.csv"],
            MimeTypes = ["csv/*"]
        };

        FilePickerOpenOptions options = new() {
            Title = "Choose csv file.",
            AllowMultiple = false,
            FileTypeFilter = [fileTypes]
        };

        var files = await _currentWindow.StorageProvider.OpenFilePickerAsync(options);

        if (files.Any() && files[0].CanBookmark)
            return (files[0].TryGetLocalPath() is string filePath) ? filePath : Option<string>.None;

        return Option<string>.None;
    }

    private static async Task<bool> FileExists(string path) => await Task.Run(() => File.Exists(path));

    private static async Task<bool> DirectoryExists(string path) => await Task.Run(() => Directory.Exists(path));

    private static async Task<Fin<List<Task<Fin<bool>>>>> BuildDirectoryList(string csvFile, string location) {
        using StreamReader thesReader = new(csvFile);
        using CsvReader thecReader = new(thesReader, CultureInfo.InvariantCulture);
        thecReader.Context.RegisterClassMap<FolderNumberHolderMap>();

        List<Task<Fin<bool>>> createDirectoryTasks = [];
        try {
            await foreach (FolderNumberHolder currentPacker in thecReader.GetRecordsAsync<FolderNumberHolder>()) {
                if (!Directory.Exists(Path.Combine(location, currentPacker.FolderNumber))) {
                    createDirectoryTasks.Add(Task.Run(() => {
                        try {
                            Directory.CreateDirectory(location + Path.DirectorySeparatorChar + currentPacker.FolderNumber);
                            return true;
                        } catch (Exception ex) {
                            return Fin.Fail<bool>(ex);
                        }
                    }));
                }
            }
            return createDirectoryTasks;
        } catch (Exception ex) {
            return Fin.Fail<List<Task<Fin<bool>>>>(ex);
        }
    }

    private static string BuildMessage(Fin<bool>[] results) {
        StringBuilder stringBuilder = new();
        foreach (Fin<bool> item in results.Where(x => x.IsFail)) {
            item.IfFail(theError => stringBuilder.AppendLine(theError.Message));
        }
        if (stringBuilder.Length < 1) {
            return "All folders created successfully.";
        } else {
            return stringBuilder.ToString();
        }
    }
}
