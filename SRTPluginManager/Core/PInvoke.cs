using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SRTPluginManager.Core
{
    public static class PInvoke
    {
        private delegate bool HandlerRoutine(uint dwCtrlEvent);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);

        public static async Task SendCtrlCAsync(this Process process)
        {
            await Task.Run(() =>
            {
                using (process)
                {
                    int pid = process.Id;

                    FreeConsole();
                    if (AttachConsole((uint)pid))
                    {
                        SetConsoleCtrlHandler(null, true); // Add null handler?
                        GenerateConsoleCtrlEvent(0, 0); // Send Ctrl+C event?
                        Thread.Sleep(2000); // Wait 2000ms.
                        FreeConsole(); // Release the attached console.
                        SetConsoleCtrlHandler(null, false); // Remove null handler?
                    }

                    process.WaitForExit();
                }
            });
        }
    }
}
