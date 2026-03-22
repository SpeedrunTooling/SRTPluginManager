using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SRTPluginManager.Properties;

namespace SRTPluginManager.Core
{
    public static class Utilities
    {
        public static readonly string ApplicationPath = AppContext.BaseDirectory;
        public static readonly string TempFolderPath = Path.Combine(ApplicationPath, "tmp");
        public static readonly string PluginFolderPath = Path.Combine(ApplicationPath, "plugins");
        public static readonly string WebSocketConfig = Path.Combine(Path.Combine(PluginFolderPath, "SRTPluginWebSocket"), "SRTPluginWebSocket.cfg");

        public static PluginConfiguration Config { get; set; }

        public static AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        public enum Platform
        {
            x86,
            x64,
            Any
        }

        public enum VersionType
        {
            None,
            Required,
            Optional
        }

        #region Config

        private static string GetConfigFile() => Path.Combine(AppContext.BaseDirectory, "SRTPluginManager.cfg");

        public static readonly JsonSerializerOptions JSO = new JsonSerializerOptions() { AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip, WriteIndented = true };

        public static T LoadConfiguration<T>(string configFile) where T : class, new()
        {
            try
            {
                FileInfo configFileInfo = new FileInfo(configFile);
                if (configFileInfo.Exists)
                    using (FileStream fs = new FileStream(configFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                        return JsonSerializer.DeserializeAsync<T>(fs, JSO).Result;
                else
                    return new T(); // File did not exist, just return a new instance.
            }
            catch
            {
                return new T(); // An exception occurred when reading the file, return a new instance.
            }
        }

        public static void SaveConfiguration<T>(T configuration, string configFile) where T : class, new()
        {
            if (configuration != null) // Only save if configuration is not null.
            {
                try
                {
                    using (FileStream fs = new FileStream(configFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete))
                        JsonSerializer.SerializeAsync<T>(fs, configuration, JSO).Wait();
                }
                catch
                {
                }
            }
        }

        #endregion

        public static Task IsRunningProcess(string file)
        {
            return Task.Run(() =>
            {
                Process process = Process.Start(file);
                while (!process.HasExited)
                {
                    Thread.Sleep(100);
                }
            });
        }

        public static async Task UpdateConfig()
        {
            Config = await GetConfigAsync();
        }

        public static void KillSRT()
        {
            Process[] processes = Process.GetProcessesByName("SRTHost64").Concat(Process.GetProcessesByName("SRTHost32")).ToArray();
            foreach (Process process in processes)
                process.Kill();
        }

        /// <summary>
        /// Downloads a file as an asynchronous operation and saves it to the specified destination file path.
        /// The method returns a tuple indicating whether the download was successful and an error message if it was not.
        /// </summary>
        /// <param name="downloadUri">The URI of the file to download.</param>
        /// <param name="destinationFilePath">The destination path including the filename to save the file to.</param>
        /// <param name="cancellationToken">(Optional) A cancellation token to abort the in-progress asynchronous operation.</param>
        /// <returns>A tuple of bool and string where the bool is true upon success, and the string is non-null and contains an error message when the bool is false.</returns>
        public static async Task<(bool Success, string? ErrorMessage)> TryDownloadFileAsync(Uri downloadUri, string destinationFilePath, CancellationToken cancellationToken)
        {
            using (var httpClient = new HttpClient())
            {
                using (var httpResponseMessage = await httpClient.GetAsync(downloadUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    if (!httpResponseMessage.IsSuccessStatusCode)
                        return (false, $"Error downloading {downloadUri}{Environment.NewLine}{Environment.NewLine}HTTP Status Code {httpResponseMessage.StatusCode:D}: {httpResponseMessage.ReasonPhrase}");

                    using (var fileStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete))
                        await httpResponseMessage.Content.CopyToAsync(fileStream, cancellationToken);
                }
            }

            return (true, null);
        }

        public static async Task DownloadManagerAsync(string fileName, Uri downloadUri, string destinationDirectoryPath, CancellationToken cancellationToken)
        {
            var zipFilePath = Path.Combine(TempFolderPath, fileName);

            var downloadFileResult = await TryDownloadFileAsync(downloadUri, zipFilePath, cancellationToken);
            if (downloadFileResult.Success)
                await UnzipPackageAsync(zipFilePath, destinationDirectoryPath, cancellationToken);
            else
                MessageBox.Show(downloadFileResult.ErrorMessage, "SRT Plugin Manager - Download", MessageBoxButton.OK, MessageBoxImage.Warning);

            autoResetEvent.Set();
        }

        public static async Task DownloadFileAsync(string pluginName, string fileName, Uri downloadUri, string destinationDirectoryPath, bool isSRT, CancellationToken cancellationToken)
        {
            var zipFilePath = Path.Combine(TempFolderPath, fileName);

            var downloadFileResult = await TryDownloadFileAsync(downloadUri, zipFilePath, cancellationToken);
            if (downloadFileResult.Success)
                await UnzipPackageAsync(pluginName, zipFilePath, destinationDirectoryPath, isSRT, cancellationToken);
            else
                MessageBox.Show(downloadFileResult.ErrorMessage, "SRT Plugin Manager - Download", MessageBoxButton.OK, MessageBoxImage.Warning);

            autoResetEvent.Set();
        }

        public static async Task UnzipPackageAsync(string zipFilePath, string destinationDirectoryPath, CancellationToken cancellationToken)
        {
            try
            {
                await using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Read))
                    await zipArchive.ExtractToDirectoryAsync(destinationDirectoryPath, true, cancellationToken);
            }
            finally
            {
                File.Delete(zipFilePath);
            }
        }

        public static async Task UnzipPackageAsync(string pluginName, string zipFilePath, string destinationDirectoryPath, bool isSRT, CancellationToken cancellationToken)
        {
            // Ensure the SRT is closed before we start trying to replace files.
            KillSRT();

            if (!isSRT)
            {
                var pluginDirectory = new DirectoryInfo(Path.Combine(destinationDirectoryPath, pluginName));
                if (pluginDirectory.Exists)
                {
                    // Save plugin config file.
                    var configFile = pluginDirectory.EnumerateFiles(string.Format("{0}.cfg", pluginName), SearchOption.TopDirectoryOnly).FirstOrDefault();

                    // If the config file exists, copy it to the temp folder until after the delete and unzip completes.
                    if (configFile != default && configFile.Exists)
                        configFile.CopyTo(Path.Combine(TempFolderPath, configFile.Name), true);

                    // Delete the plugin folder to ensure a clean slate.
                    pluginDirectory.Delete(true);
                }
            }

            await UnzipPackageAsync(zipFilePath, destinationDirectoryPath, cancellationToken);

            if (!isSRT)
            {
                // If the plugin config file existed in the temp folder, copy it back to the plugin's folder now and then delete it from the temp folder.
                if (File.Exists(Path.Combine(TempFolderPath, string.Format("{0}.cfg", pluginName))))
                {
                    File.Copy(Path.Combine(TempFolderPath, string.Format("{0}.cfg", pluginName)), Path.Combine(destinationDirectoryPath, pluginName, string.Format("{0}.cfg", pluginName)));
                    File.Delete(Path.Combine(TempFolderPath, string.Format("{0}.cfg", pluginName)));
                }
            }
        }

        public static void UninstallExtension(string source, string extensionName)
        {
            var dirs = Directory.GetDirectories(source);
            foreach (string directory in dirs)
            {
                if (extensionName == Path.GetFileName(directory))
                {
                    Directory.Delete(directory, true);
                }
            }
        }

        public static void DeleteFiles(string source)
        {
            var files = Directory.GetFiles(source);
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        public static string GetFileVersionInfo(string folder, string file)
        {
            var filePath = Path.Combine(folder, file);
            bool results = File.Exists(filePath);
            if (results)
            {
                return FileVersionInfo.GetVersionInfo(filePath).FileVersion;
            }
            return "0.0.0.0";
        }

        public static async Task<PluginConfiguration> GetConfigAsync()
        {
            using (HttpClient httpClient = new HttpClient())
            using (Stream stream = await httpClient.GetStreamAsync(@"https://raw.githubusercontent.com/SpeedrunTooling/SRTPlugins/main/SRTPluginManager.cfg"))
                return await JsonSerializer.DeserializeAsync<PluginConfiguration>(stream, JSO);
        }

        public static bool IsUpdated(string current, string latest)
        {
            var currentSplit = current.Split('.');
            var latestSplit = latest.Split('.');

            for (var i = 0; i < currentSplit.Length; i++)
            {
                if (int.Parse(currentSplit[i]) > int.Parse(latestSplit[i]))
                {
                    return true;
                }
                if (int.Parse(currentSplit[i]) < int.Parse(latestSplit[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static object GetSetting(string setting)
        {
            return Settings.Default[setting];
        }

        public static void UpdateSetting(string setting, string value)
        {
            Settings.Default[setting] = value;
            Settings.Default.Save();
        }

        public static void LogInfoWriteLine(this StreamWriter sw, string message)
        {
            sw.WriteLine(string.Format("[INFO]: {0}", message));
        }
        public static void LogInfoWriteLine(this StreamWriter sw, string format, params object?[] args) => LogInfoWriteLine(sw, string.Format(format, args));

        public static void LogDebugWriteLine(this StreamWriter sw, string message)
        {
#if DEBUG
            sw.WriteLine(string.Format("[DEBUG]: {0}", message));
#endif
        }
        public static void LogDebugWriteLine(this StreamWriter sw, string format, params object?[] args) => LogDebugWriteLine(sw, string.Format(format, args));
    }
}
