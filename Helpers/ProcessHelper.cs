using System;
using System.Diagnostics;

namespace WD2ModBundler.Helpers
{
    public static class ProcessHelper
    {
        public static void RunProcess(string fileName, string arguments, string? workingDirectory = null)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = workingDirectory ?? ""
            };

            Process? process = Process.Start(psi);
            if (process == null)
                throw new InvalidOperationException($"Failed to start process: {fileName}");

            process.WaitForExit();
        }
    }
}