using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Folder_Creator.Services
{
    public class FileAccessService : IFileAccessProvider
    {
        public string GetDestination(string destinationFile)
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
                    List<string> packers = LoadPackers(spreadsheetFile).ToList();
                    for (int i = 1; i < packers.Count; i++)
                    {
                        if (!Directory.Exists(Path.Combine(location, packers[i])))
                        {
                            Directory.CreateDirectory(location + Path.DirectorySeparatorChar + packers[i]);
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
        public async Task<string> GetDestinationAsync(string destinationFile)
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
                    List<string> packers = await LoadPackersAsync(spreadsheetFile);
                    for (int i = 1; i < packers.Count; i++)
                    {
                        if (!Directory.Exists(Path.Combine(location, packers[i])))
                        {
                            await Task.Run(() => Directory.CreateDirectory(location + Path.DirectorySeparatorChar + packers[i]));
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
        private static IEnumerable<string> LoadPackers(string fileName)
        {
            List<string> columns = new List<string>();
            using (CsvFileReader theReader = new CsvFileReader(File.OpenRead(fileName)))
            {
                while (theReader.ReadRow(columns))
                {
                    yield return columns[0];
                }
            }
        }
        private static async Task<List<string>> LoadPackersAsync(string fileName)
        {
            List<string> cells = new List<string>();
            List<string> packersData = new List<string>();
            using (CsvFileReader theReader = new CsvFileReader(File.OpenRead(fileName)))
            {
                while (await theReader.ReadRowAsync(cells))
                {
                    packersData.Add(cells[0]);
                }
            }

            return packersData;
        }
    }
}
