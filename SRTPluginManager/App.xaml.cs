using static SRTPluginManager.Core.Utilities;
using System.Windows;
using System.Linq;
using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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
#if DEBUG
            logFS = new FileStream(string.Format(@"SRTPluginManager_{0}.log", DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-fffffff")), FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
#else
            logFS = new FileStream(@"SRTPluginManager.log", FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
#endif
            logSW = new StreamWriter(logFS, Encoding.UTF8) { AutoFlush = true };

            logSW.LogInfoWriteLine("Evaluating command-line arguments...");
            // For each command-line argument, do things?
            foreach (string[] arg in e.Args.Select(a => a.Split('=', StringSplitOptions.RemoveEmptyEntries)))
            {
                // --LoadUpdate is called when a new SRTPluginManager.exe has been extracted to /tmp.
                // In theory, if this parameter is called, we're the /tmp program and we need to copy ourselves down a folder. For sanity sake, verify that we're running from within /tmp.
                if (string.Equals("--LoadUpdate", arg[0], StringComparison.InvariantCultureIgnoreCase))
                {
                    logSW.LogInfoWriteLine("Handling --LoadUpdate argument...");
                    DirectoryInfo baseDirectory = new DirectoryInfo(AppContext.BaseDirectory); // Get the current folder we're running in.
                    if (string.Equals("tmp", baseDirectory.Name, StringComparison.InvariantCultureIgnoreCase)) // Verify we're running from /tmp.
                    {
                        logSW.LogDebugWriteLine("We're inside tmp.");
                        Process currentProcess = Process.GetCurrentProcess(); // Get our current process object.
                        FileInfo processFile = new FileInfo(currentProcess.MainModule.FileName); // Get the full path to our exe.
                        string newPath = Path.Combine(processFile.Directory.Parent.FullName, processFile.Name); // Get the path where our new exe will reside.
                        logSW.LogDebugWriteLine(string.Format("{0} = \"{1}\"", nameof(newPath), newPath));

                        Task.Delay(2000).Wait(); // Wait 2 seconds for the callee to exit.
                        foreach (FileInfo file in baseDirectory.EnumerateFiles("*", SearchOption.TopDirectoryOnly))
                        {
                            string destPath = Path.Combine(file.Directory.Parent.FullName, file.Name);
                            logSW.LogDebugWriteLine("Copying file \"{0}\" to \"{1}\"...", file.FullName, destPath);
                            file.CopyTo(destPath, true); // Copy the file down a folder, overwriting what already exists there.
                        }

                        logSW.LogDebugWriteLine("Starting process...");
                        Process.Start(newPath); // Start the new version.

                        logSW.LogDebugWriteLine("Exiting...");
                        Environment.Exit(0); // Exit gracefully.
                    }
                }
            }

            DirectoryInfo tmpFolder;
            if ((tmpFolder = new DirectoryInfo(TempFolderPath)).Exists)
            {
                logSW.LogDebugWriteLine("Deleting tmp folder to clear it out, it'll be recreated anyways...");
                tmpFolder?.Delete(true);
            }

            logSW.LogInfoWriteLine("Getting latest SRTPlugins config...");
            Task.Run(async () => await UpdateConfig()).Wait(); // Get latest config from github.
        }
    }
}
