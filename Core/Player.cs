using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace PortalStillAlive.Core;

public static class Player
{
    public static void Play(string fileName)
    {
        if (OperatingSystem.IsWindows())
        {
            PlayOnWindows(fileName);
        }
        else if (OperatingSystem.IsLinux())
        {
            PlayOnLinux(fileName);
        }
        else
        {
            throw new PlatformNotSupportedException("only Linux or Windows");
        }
    }

    #region windows

    [SupportedOSPlatform("windows")]
    private static void PlayOnWindows(string fileName)
    {
        MciSendString("Close All");
        MciSendString($"Play {fileName}");
    }

    [SupportedOSPlatform("windows")]
    private static void MciSendString(string command)
    {
        uint res = mciSendStringW(command, IntPtr.Zero, 1024 * 1024, IntPtr.Zero);
        if (res != 0)
        {
            string errStr = $"Error executing MCI command '{command}'. Error code: {res}. Error Msg: ";
            var buffer = new StringBuilder(128);
            _ = mciGetErrorStringW(res, buffer, 128);
            throw new Exception(errStr + buffer);
        }
    }

    [SupportedOSPlatform("windows")]
    [DllImport("winmm.dll", CharSet = CharSet.Unicode)]
    private static extern uint mciSendStringW(
        string lpszCommand,
        IntPtr lpszReturnString,
        uint cchReturn,
        IntPtr hwndCallback
    );

    [SupportedOSPlatform("windows")]
    [DllImport("winmm.dll", CharSet = CharSet.Unicode)]
    private static extern uint mciGetErrorStringW(
        uint fdwError,
        StringBuilder lpszErrorText,
        uint cchErrorText
    );

    #endregion

    #region linux

    [SupportedOSPlatform("linux")]
    private static void PlayOnLinux(string fileName)
    {
        EnsureMpg123IsAvailable();

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "mpg123",
            Arguments = $"-q {fileName}",
            UseShellExecute = false,
            CreateNoWindow = true
        });
        if (process == null)
        {
            throw new Exception("play failed");
        }
    }

    [SupportedOSPlatform("linux")]
    private static void EnsureMpg123IsAvailable()
    {
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "which",
            Arguments = "mpg123",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        });
        if (process == null) return;
        process.WaitForExit();
        string output = process.StandardOutput.ReadToEnd().Trim();
        bool exist = !string.IsNullOrEmpty(output);
        if (!exist)
        {
            throw new Exception("mpg123 not found\nUse the \"sudo apt install mpg123\" to install");
        }
    }

    #endregion
}
