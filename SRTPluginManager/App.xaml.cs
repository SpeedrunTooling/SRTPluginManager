using static SRTPluginManager.Core.Utilities;
using System.Windows;
using System.Linq;
using System;
using System.IO;
using System.Diagnostics;

namespace SRTPluginManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await UpdateConfig(); // Get latest config from github.

            // For each command-line argument, do things?
            foreach (string[] arg in e.Args.Select(a => a.Split('=', StringSplitOptions.RemoveEmptyEntries)))
            {
                // --LoadUpdate is called when a new SRTPluginManager.exe has been extracted to /tmp.
                // In theory, if this parameter is called, we're the /tmp program and we need to copy ourselves down a folder. For sanity sake, verify that we're running from within /tmp.
                if (string.Equals("--LoadUpdate", arg[0], StringComparison.InvariantCultureIgnoreCase))
                {
                    DirectoryInfo baseDirectory = new DirectoryInfo(AppContext.BaseDirectory); // Get the current folder we're running in.
                    if (string.Equals("tmp", baseDirectory.Name, StringComparison.InvariantCultureIgnoreCase)) // Verify we're running from /tmp.
                    {
                        Process currentProcess = Process.GetCurrentProcess(); // Get our current process object.
                        FileInfo processFile = new FileInfo(currentProcess.MainModule.FileName); // Get the full path to our exe.
                        string newPath = Path.Combine(processFile.Directory.Parent.FullName, processFile.Name); // Get the path where we intend to copy our exe to.
                        processFile.CopyTo(newPath, true); // Copy the exe.
                        Process.Start(newPath); // Start the new version.
                        Environment.Exit(0); // Exit gracefully.
                    }
                }
            }


        }
    }
}
