﻿using SRTPluginManager.Properties;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using static SRTPluginBase.Extensions;

namespace SRTPluginManager.Core
{
    public class Utilities
    {
        public static readonly string ApplicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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
            var i = 1;
            foreach (string file in files)
            {
                File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);
                File.Delete(file);
                i++;
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
    }
}
