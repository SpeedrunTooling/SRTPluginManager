using SRTPluginManager.Properties;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System;
using System.Linq;

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

        public static async Task DownloadManagerAsync(string fileName, string url, Button button, string destination, CancellationToken cancellationToken)
        {
            var file = Path.Combine(TempFolderPath, fileName);

            // Download file.
            using (var httpClient = new HttpClient())
            using (var downloadStream = await httpClient.GetStreamAsync(url))
            using (var fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete))
                await downloadStream.CopyToAsync(fileStream);

            // Unzip file.
            await UnzipPackageAsync(file, destination, cancellationToken);
            autoResetEvent.Set();
        }

        public static async Task DownloadFileAsync(string pluginName, string fileName, Uri downloadUri, Button button, string destination, bool isSRT, CancellationToken cancellationToken)
        {
            var file = Path.Combine(TempFolderPath, fileName);

            // Download file.
            using (var httpClient = new HttpClient())
            using (var downloadStream = await httpClient.GetStreamAsync(downloadUri, cancellationToken))
            using (var fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete))
                await downloadStream.CopyToAsync(fileStream, cancellationToken);

            // Unzip file.
            await UnzipPackageAsync(pluginName, file, destination, isSRT, cancellationToken);
            autoResetEvent.Set();
        }

        public static async Task UnzipPackageAsync(string file, string destination, CancellationToken cancellationToken)
        {
            try
            {
                await using (var zipArchive = ZipFile.Open(file, ZipArchiveMode.Read))
                    await zipArchive.ExtractToDirectoryAsync(destination, true, cancellationToken);
            }
            finally
            {
                File.Delete(file);
            }
        }

        public static async Task UnzipPackageAsync(string pluginName, string file, string destination, bool isSRT, CancellationToken cancellationToken)
        {
            // Ensure the SRT is closed before we start trying to replace files.
            KillSRT();

            if (!isSRT)
            {
                var pluginDirectory = new DirectoryInfo(Path.Combine(destination, pluginName));
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

            try
            {
                await using (var zipArchive = ZipFile.Open(file, ZipArchiveMode.Read))
                    await zipArchive.ExtractToDirectoryAsync(destination, true, cancellationToken);
            }
            finally
            {
                File.Delete(file);
            }

            if (!isSRT)
            {
                // If the plugin config file existed in the temp folder, copy it back to the plugin's folder now and then delete it from the temp folder.
                if (File.Exists(Path.Combine(TempFolderPath, string.Format("{0}.cfg", pluginName))))
                {
                    File.Copy(Path.Combine(TempFolderPath, string.Format("{0}.cfg", pluginName)), Path.Combine(destination, pluginName, string.Format("{0}.cfg", pluginName)));
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
