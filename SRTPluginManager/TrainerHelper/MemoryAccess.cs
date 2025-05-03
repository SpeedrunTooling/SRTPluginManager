using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace SRTPluginManager.TrainerHelper
{
    public class MemoryAccess
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint size, out int lpNumberOfBytesRead);

        private const uint PROCESS_ALL_ACCESS = 0x1F0FFF;

        private IntPtr processHandle;

        /// <summary>
        /// Attaches to a process by its name.
        /// </summary>
        public bool AttachToProcess(string processName)
        {
            var process = Process.GetProcessesByName(processName).FirstOrDefault();
            if (process == null)
            {
                return false;
            }
            processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
            return processHandle != IntPtr.Zero;
        }


        /// <summary>
        /// Writes a value to the specified memory address.
        /// </summary>
        public void WriteMemoryBytes(IntPtr address, byte[] bytes)
        {
            if (processHandle == IntPtr.Zero)
                throw new InvalidOperationException("Process is not attached.");

            bool success = WriteProcessMemory(processHandle, address, bytes, (uint)bytes.Length, out int bytesWritten);

            if (!success || bytesWritten != bytes.Length)
            {
                throw new InvalidOperationException($"Failed to write memory at address {address.ToString("X")}. Bytes written: {bytesWritten}");
            }
        }

        /// <summary>
        /// Writes a specific type of value to memory.
        /// </summary>
        public void WriteMemory<T>(IntPtr address, T value) where T : unmanaged
        {
            byte[] bytes = ConvertToBytes(value);
            WriteMemoryBytes(address, bytes);
        }

        /// <summary>
        /// Reads a value from the specified memory address.
        /// </summary>
        public T ReadMemory<T>(IntPtr address) where T : unmanaged
        {
            if (processHandle == IntPtr.Zero)
                throw new InvalidOperationException("Process is not attached.");

            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];

            bool success = ReadProcessMemory(processHandle, address, buffer, (uint)size, out int bytesRead);

            if (!success || bytesRead != size)
            {
                throw new InvalidOperationException($"Failed to read memory at address {address.ToString("X")}.");
            }

            return ConvertFromBytes<T>(buffer);
        }

        /// <summary>
        /// Converts a value to a byte array.
        /// </summary>
        private byte[] ConvertToBytes<T>(T value) where T : unmanaged
        {
            if (typeof(T) == typeof(short))
                return BitConverter.GetBytes(Convert.ToInt16(value));
            if (typeof(T) == typeof(int))
                return BitConverter.GetBytes(Convert.ToInt32(value));
            if (typeof(T) == typeof(float))
                return BitConverter.GetBytes(Convert.ToSingle(value));

            throw new NotSupportedException($"Type {typeof(T)} is not supported.");
        }

        /// <summary>
        /// Converts a byte array to a value of type T.
        /// </summary>
        private T ConvertFromBytes<T>(byte[] bytes) where T : unmanaged
        {
            if (typeof(T) == typeof(short))
                return (T)(object)BitConverter.ToInt16(bytes, 0);
            if (typeof(T) == typeof(int))
                return (T)(object)BitConverter.ToInt32(bytes, 0);
            if (typeof(T) == typeof(float))
                return (T)(object)BitConverter.ToSingle(bytes, 0);

            throw new NotSupportedException($"Type {typeof(T)} is not supported.");
        }
    }
}
