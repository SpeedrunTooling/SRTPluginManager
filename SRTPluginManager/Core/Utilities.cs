using SRTPluginManager.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Xml.Serialization;
using static SRTPluginBase.Extensions;
using System.Net.Http;

namespace SRTPluginManager.Core
{
    public class Utilities
    {
        public static readonly string ApplicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static readonly string TempFolderPath = Path.Combine(ApplicationPath, "tmp");
        public static readonly string PluginFolderPath = Path.Combine(ApplicationPath, "plugins");
        public static readonly string WebSocketConfig = Path.Combine(Path.Combine(PluginFolderPath, "SRTPluginWebSocket"), "SRTPluginWebSocket.cfg");
        public static readonly string srtHostURL = "https://api.github.com/repos/Squirrelies/SRTHost/releases/latest";
        public static readonly string dotNetCorePath = @"C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App";
        public static readonly string dotNetCore32Path = @"C:\Program Files (x86)\dotnet\shared\Microsoft.WindowsDesktop.App";
        public static readonly string dotNetCore32URL = "https://download.visualstudio.microsoft.com/download/pr/f703f604-a973-4ab9-abe4-b4b2ec786e66/af8cea0988953ef074157ea99d30879a/windowsdesktop-runtime-3.1.16-win-x86.exe";
        public static readonly string dotNetCore64URL = "https://download.visualstudio.microsoft.com/download/pr/7cea63ad-1e76-41f0-a54a-eacb48fec749/87c339835cd7647c0fee3f14820cd909/windowsdesktop-runtime-3.1.16-win-x64.exe";
        public static readonly string vgrSource = "pack://application:,,,/SRTPluginManager;component/Images/VGR.png";
        public static readonly string squirrelSource = "pack://application:,,,/SRTPluginManager;component/Images/Squirrelies.png";
        public static readonly string willowSource = "pack://application:,,,/SRTPluginManager;component/Images/Willow.png";
        public static readonly string mysterionSource = "pack://application:,,,/SRTPluginManager;component/Images/Mysterion06.png";
        public static readonly string toastSource = "pack://application:,,,/SRTPluginManager;component/Images/CursedToast.png";
        public static readonly string kapdapSource = "pack://application:,,,/SRTPluginManager;component/Images/Kapdap.png";

        public static PluginConfiguration Config { get; set; }

        public static AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        #region Config
        //private static string GetConfigFile(Assembly a) => Path.Combine(new FileInfo(a.Location).DirectoryName, string.Format("{0}.cfg", Path.GetFileNameWithoutExtension(new FileInfo(a.Location).Name)));
        //
        //private static JsonSerializerOptions jso = new JsonSerializerOptions() { AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip, WriteIndented = true };
        //public static T LoadConfiguration<T>() where T : class, new() => LoadConfiguration<T>(GetConfigFile(Assembly.GetCallingAssembly()));
        //public static T LoadConfiguration<T>(string configFile) where T : class, new()
        //{
        //    try
        //    {
        //        FileInfo configFileInfo = new FileInfo(configFile);
        //        if (configFileInfo.Exists)
        //            using (FileStream fs = new FileStream(configFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
        //                return JsonSerializer.DeserializeAsync<T>(fs, jso).Result;
        //        else
        //            return new T(); // File did not exist, just return a new instance.
        //    }
        //    catch
        //    {
        //        return new T(); // An exception occurred when reading the file, return a new instance.
        //    }
        //}
        //public static void SaveConfiguration<T>(T configuration) where T : class, new() => SaveConfiguration<T>(configuration, GetConfigFile(Assembly.GetCallingAssembly()));
        //public static void SaveConfiguration<T>(T configuration, string configFile) where T : class, new()
        //{
        //    if (configuration != null) // Only save if configuration is not null.
        //    {
        //        try
        //        {
        //            using (FileStream fs = new FileStream(configFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete))
        //                JsonSerializer.SerializeAsync<T>(fs, configuration, jso).Wait();
        //        }
        //        catch
        //        {
        //        }
        //    }
        //}

        #endregion

        public enum Platform
        {
            x86,
            x64
        }

        public enum Plugins
        {
            SRTPluginProviderRE0,
            SRTPluginProviderRE1C,
            SRTPluginProviderRE1,
            SRTPluginProviderRE2C,
            SRTPluginProviderRE2,
            SRTPluginProviderRE3C,
            SRTPluginProviderRE3,
            SRTPluginProviderRE4,
            SRTPluginProviderRE5,
            SRTPluginProviderRE6,
            SRTPluginProviderRE7,
            SRTPluginProviderRE8,
            SRTPluginProviderRECVX,
        }

        public enum VersionType
        {
            None,
            Required,
            Optional
        }

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

        public static void UpdateConfig(bool isManual = false)
        {
            if (File.Exists(Assembly.GetCallingAssembly().GetConfigFile()))
            {
                Config = LoadConfiguration<PluginConfiguration>();
            }
            else
            {
                Config = new PluginConfiguration();
            }
            GetHostVersion(isManual);
            GetPluginVersions(isManual);
            GetExtensionVersions(isManual);
        }

        public static void GetExtensionVersions(bool isManual)
        {
            var setting = isManual ? "ExtensionUpdate2" : "ExtensionUpdate";
            if (IsUpdatedTimestamp(setting))
            {
                string[] names = { "SRTPluginUIJSON", "SRTPluginWebSocket" };
                for (var i = 0; i < names.Length; i++)
                {
                    Config.ExtensionsConfig[i].pluginName = names[i];
                    var version = GetVersionInfo(Config.ExtensionsConfig[i].tagURL, true);
                    Config.ExtensionsConfig[i].currentVersion = version.ToString();
                }
                Config.SaveConfiguration();
            }
        }

       public static void DownloadFile(string fileName, string url, Button button)
       {
            var file = Path.Combine(TempFolderPath, fileName);
            using (var wc = new WebClient())
            {
                wc.DownloadFileCompleted += async (s, ev) =>
                {
                    await IsRunningProcess(file);
                    button.Visibility = Visibility.Collapsed;
                    File.Delete(file);
                };
                wc.DownloadFileAsync(new Uri(url), file);
            }
        }

        public static async Task DownloadFileAsync(string fileName, string url, Button button, string destination, bool isSRT = false)
        {
            var file = Path.Combine(TempFolderPath, fileName);

            // Download file.
            using (var hc = new HttpClient())
            using (var s = await hc.GetStreamAsync(url))
            using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete))
                await s.CopyToAsync(fs);

            // Unzip file.
            UnzipPackage(file, destination, isSRT);
            autoResetEvent.Set();
        }

        public static void GetPluginVersions(bool isManual)
        {
            var setting = isManual ? "LastUpdate2" : "LastUpdate";
            if (IsUpdatedTimestamp(setting))
            {
                string[] names = { "SRTPluginProviderRE0", "SRTPluginProviderRE1C", "SRTPluginProviderRE1", "SRTPluginProviderRE2C", "SRTPluginProviderRE2", "SRTPluginProviderRE3C", "SRTPluginProviderRE3", "SRTPluginProviderRE4", "SRTPluginProviderRE5", "SRTPluginProviderRE6", "SRTPluginProviderRE7", "SRTPluginProviderRE8", "SRTPluginProviderRECVX" };
                for (var i = 0; i < names.Length; i++)
                {
                    Config.PluginConfig[i].pluginName = names[i];
                    var version = GetVersionInfo(Config.PluginConfig[i].tagURL, true);
                    Config.PluginConfig[i].currentVersion = version.ToString();
                }
                Config.SaveConfiguration();
            }
        }

        public static string GetHostVersion(bool isManual)
        {
            var setting = isManual ? "SRTUpdate2" : "SRTUpdate";
            if (IsUpdatedTimestamp(setting))
            {
                Config.SRTConfig.currentVersion = GetVersionInfo(srtHostURL).ToString();
                Config.SaveConfiguration();
            }
            return Config.SRTConfig.currentVersion;
        }

        public static void UnzipPackage(string file, string destination, bool isSRT)
        {
            if (!Directory.Exists(file))
            {
                ZipFile.ExtractToDirectory(file, TempFolderPath);
            }

            if (isSRT)
            {
                File.Delete(file);
                UpdateSRTPackage(TempFolderPath, destination);
            }
            else
            {
                var dirs = Directory.GetDirectories(TempFolderPath);
                UpdatePackage(dirs[0], destination);
            }
            
        }

        public static void UpdateSRTPackage(string source, string destination)
        {
            var filesSource = Directory.GetFiles(source);
            CopyTmpFiles(filesSource, destination);
            var directoriesSource = Directory.GetDirectories(source);
            if (directoriesSource.Length > 0) CopyTmpFolders(directoriesSource, destination, source);
            DeleteTmpFiles();
        }

        public static void UpdatePackage(string source, string destination)
        {
            var dest = Path.Combine(destination, Path.GetFileName(source));
            if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);
            var filesSource = Directory.GetFiles(source);
            CopyTmpFiles(filesSource, dest);
            var directoriesSource = Directory.GetDirectories(source);
            if (directoriesSource.Length > 0) CopyTmpFolders(directoriesSource, dest, source);
            DeleteTmpFiles();
        }

        public static void CopyPluginProvider(string source, string destination)
        {
            var dest = Path.Combine(destination, Path.GetFileName(source));
            if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);
            var filesSource = Directory.GetFiles(source);
            CopyFiles(filesSource, dest);
        }

        private static void CopyTmpFolders(string[] folders, string destination, string source)
        {
            foreach (string folder in folders)
            {
                var folderPath = Path.Combine(destination, Path.GetFileName(folder));
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                var files = Directory.GetFiles(folder);
                CopyTmpFiles(files, folderPath);
            }
            DeleteDirectories(source);
        }

        private static void CopyTmpFiles(string[] files, string destination)
        {
            foreach (string file in files)
            {
                File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);
                File.Delete(file);
            } 
        }

        private static void CopyFiles(string[] files, string destination)
        {
            foreach (string file in files)
            {
                File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);
            }
        }

        private static void DeleteTmpFiles()
        {
            DeleteDirectories(TempFolderPath);
            DeleteFiles(TempFolderPath);
        }

        public static void DeleteDirectories(string source)
        {
            var dirs = Directory.GetDirectories(source);
            foreach (string directory in dirs)
            {
                Directory.Delete(directory, true);
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

        private static void CopyPluginFolder(string[] folders, string source, string destination)
        {
            foreach (string folder in folders)
            {
                var folderPath = Path.Combine(destination, Path.GetFileName(folder));
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                var files = Directory.GetFiles(folder);
                CopyTmpFiles(files, folderPath);
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

        public static VersionType CheckDotNetCore(int required)
        {
            string[] dirs = Directory.GetDirectories(dotNetCorePath);
            Array.Reverse(dirs);
            string[] folders;
            string[] split;

            foreach (string dir in dirs)
            {
                folders = dir.Split('\\');
                split = folders[folders.Length - 1].Split('.');
                if (split[0] != "3" || split[1] != "1")
                {
                    continue;
                }
                else if (split[0] == "3" || split[1] == "1")
                {
                    if (int.Parse(split[2]) >= required) { return VersionType.Required; }
                }
            }
            return VersionType.None;
        }

        public static VersionType CheckDotNetCore(int required, int optional)
        {
            string[] dirs = Directory.GetDirectories(dotNetCore32Path);
            Array.Reverse(dirs);
            string[] folders;
            string[] split;

            foreach (string dir in dirs)
            {
                folders = dir.Split('\\');
                split = folders[folders.Length - 1].Split('.');
                if (split[0] != "3" || split[1] != "1")
                {
                    continue;
                }
                else if (split[0] == "3" || split[1] == "1")
                {
                    if (int.Parse(split[2]) == required) { return VersionType.Required; }
                    if (int.Parse(split[2]) == optional) { return VersionType.Optional; }
                }
            }
            return VersionType.None;
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

        public static object GetVersionInfo(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "SpeedrunToolHost";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Dictionary<string, object> test = JsonSerializer.DeserializeAsync<Dictionary<string, object>>(Stream.Synchronized(response.GetResponseStream()), JSO).Result;
            return test["tag_name"];
        }

        public static object GetVersionInfo(string url, bool isPluginProvider)
        {
            if (url == null) { return "0.0.0.0"; }
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "SpeedrunToolHost";
            HttpWebResponse response;

            try { response = (HttpWebResponse)request.GetResponse(); }
            catch { return "0.0.0.0"; }

            Dictionary<string, object> version = JsonSerializer.DeserializeAsync<Dictionary<string, object>>(Stream.Synchronized(response.GetResponseStream()), JSO).Result;
            return version["tag_name"];
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

        public static long GetTimestamp()
        {
            return DateTime.UtcNow.Ticks;
        }

        public static bool IsUpdatedTimestamp(string timestamp)
        {
            var previousTime = Settings.Default[timestamp];
            long currentTime = DateTime.UtcNow.Ticks;
            long elapsedTicks = currentTime - (long)previousTime;
            TimeSpan timeSpan = new TimeSpan(elapsedTicks);

            if (timeSpan.TotalSeconds >= 3600)
            {
                Settings.Default[timestamp] = currentTime;
                Settings.Default.Save();
                return true;
            }
            return false;
        }

        public static object GetSetting(string setting)
        {
            return Settings.Default[setting];
        }

        public static void UpdateSetting(string setting, bool onoff)
        {
            Settings.Default[setting] = onoff;
            Settings.Default.Save();
        }

        public static void UpdateSetting(string setting, string value)
        {
            Settings.Default[setting] = value;
            Settings.Default.Save();
        }
    }
}
