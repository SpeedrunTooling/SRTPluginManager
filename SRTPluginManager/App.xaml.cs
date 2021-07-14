﻿using static SRTPluginManager.Core.Utilities;
using System.Windows;
using System.Linq;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text;

namespace SRTPluginManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static FileStream logFS;
        public static StreamWriter logSW;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string dateTimeString = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-fffffff");
            using (logFS = new FileStream(string.Format(@"SRTPluginManager_{0}.log", dateTimeString), FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete))
            using (logSW = new StreamWriter(logFS, Encoding.UTF8) { AutoFlush = true })
            {
                logSW.WriteLine("Getting latest SRTPlugins config...");
                Task.Run(async () => await UpdateConfig()).Wait(); // Get latest config from github.

                logSW.WriteLine("Evaluating command-line arguments...");
                // For each command-line argument, do things?
                foreach (string[] arg in e.Args.Select(a => a.Split('=', StringSplitOptions.RemoveEmptyEntries)))
                {
                    // --LoadUpdate is called when a new SRTPluginManager.exe has been extracted to /tmp.
                    // In theory, if this parameter is called, we're the /tmp program and we need to copy ourselves down a folder. For sanity sake, verify that we're running from within /tmp.
                    if (string.Equals("--LoadUpdate", arg[0], StringComparison.InvariantCultureIgnoreCase))
                    {
                        logSW.WriteLine("Handling --LoadUpdate argument...");
                        DirectoryInfo baseDirectory = new DirectoryInfo(AppContext.BaseDirectory); // Get the current folder we're running in.
                        if (string.Equals("tmp", baseDirectory.Name, StringComparison.InvariantCultureIgnoreCase)) // Verify we're running from /tmp.
                        {
                            logSW.WriteLine("\t[Debug] We're inside tmp.");
                            Process currentProcess = Process.GetCurrentProcess(); // Get our current process object.
                            FileInfo processFile = new FileInfo(currentProcess.MainModule.FileName); // Get the full path to our exe.
                            string newPath = Path.Combine(processFile.Directory.Parent.FullName, processFile.Name); // Get the path where we intend to copy our exe to.
                            logSW.WriteLine(string.Format("\t[Debug] {0} = \"{1}\"", nameof(newPath), newPath));
                            Task.Delay(2000).Wait(); // Wait 2 seconds for the callee to exit.
                            processFile.CopyTo(newPath, true); // Copy the exe.
                            logSW.WriteLine("\t[Debug] Starting process...");
                            Process.Start(newPath); // Start the new version.
                            logSW.WriteLine("\t[Debug] exiting...");
                            Environment.Exit(0); // Exit gracefully.
                        }
                    }
                }

                FileInfo srtPluginManagerExeTmp;
                if ((srtPluginManagerExeTmp = new FileInfo(Path.Combine(TempFolderPath, "SRTPluginManager.exe"))).Exists)
                {
                    logSW.WriteLine("\t[Debug] Deleting tmp version of SRTPluginManager.exe...");
                    srtPluginManagerExeTmp?.Delete();
                }
            }
        }
    }
}
