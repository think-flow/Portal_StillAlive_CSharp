using System.Diagnostics;
using System.Runtime.Versioning;
using NetCoreAudio;
using PortalStillAlive.Data;

namespace PortalStillAlive;

internal class Program
{
    private static void Main(string[] args)
    {
        if (OperatingSystem.IsLinux())
        {
            EnsureMpg123IsAvailable();
        }

        bool enableSound = !args.Contains("--no-sound");
        if (enableSound)
        {
            if (!File.Exists("./sa1.mp3"))
            {
                Console.WriteLine("sa1.mp3 not found");
                Environment.Exit(2);
            }
        }

        Console.CancelKeyPress += (_, e) =>
        {
            ConsoleHelper.EndDraw();
            Console.WriteLine("Got it! Exiting...");
            Environment.Exit(1);
        };

        ConsoleHelper.BeginDraw();
        ConsoleHelper.Clear();
        ConsoleHelper.DrawFrame();
        ConsoleHelper.Move(2, 2);
        Thread.Sleep(1000);

        var stopwatch = Stopwatch.StartNew();
        int currentLyric = 0;
        int x = 0;
        int y = 0;

        var lyrics = LyricData.Lyrics;

        while (lyrics[currentLyric].Mode != 9)
        {
            long pastTime = stopwatch.ElapsedMilliseconds / 10;

            if (pastTime > lyrics[currentLyric].Time)
            {
                int wordCount = 0;
                double interval;

                if (lyrics[currentLyric].Mode <= 1 || lyrics[currentLyric].Mode >= 5)
                {
                    wordCount = lyrics[currentLyric].Words.ToString()!.Length;
                }

                if (wordCount == 0)
                {
                    wordCount = 1;
                }

                if (lyrics[currentLyric].Interval < 0)
                {
                    interval = (lyrics[currentLyric + 1].Time - lyrics[currentLyric].Time) / 100d / wordCount;
                }
                else
                {
                    interval = lyrics[currentLyric].Interval / wordCount;
                }

                if (lyrics[currentLyric].Mode == 0)
                {
                    x = ConsoleHelper.DrawLyrics(lyrics[currentLyric].Words.ToString()!, x, y, interval, true);
                    y += 1;
                }
                else if (lyrics[currentLyric].Mode == 1)
                {
                    x = ConsoleHelper.DrawLyrics(lyrics[currentLyric].Words.ToString()!, x, y, interval, false);
                }
                else if (lyrics[currentLyric].Mode == 2)
                {
                    ConsoleHelper.DrawAsciiArts((int) lyrics[currentLyric].Words);
                    ConsoleHelper.Move(x + 2, y + 2);
                }
                else if (lyrics[currentLyric].Mode == 3)
                {
                    ConsoleHelper.ClearLyrics();
                    x = 0;
                    y = 0;
                }
                else if (lyrics[currentLyric].Mode == 4)
                {
                    if (enableSound)
                    {
                        var player = new Player();
                        player.Play("./sa1.mp3");
                    }
                }
                else if (lyrics[currentLyric].Mode == 5)
                {
                    ConsoleHelper.DrawCredits();
                }

                currentLyric++;
            }

            Thread.Sleep(10);
        }

        ConsoleHelper.EndDraw();
    }

    [SupportedOSPlatform("linux")]
    private static void EnsureMpg123IsAvailable()
    {
        try
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
                Console.WriteLine("mpg123 command not found");
                Console.WriteLine("""Use the "sudo apt install mpg123" to install""");
                Environment.Exit(1);
            }
        }
        catch
        {
            // ignored
        }
    }
}
