using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using PortalStillAlive.Data;

namespace PortalStillAlive.Core;

public class Stage
{
    public const string SoundFilePath = "./sa1.mp3";

    public Stage()
    {
        bool enableSound = !Environment.GetCommandLineArgs().Contains("--no-sound");
        if (enableSound)
        {
            if (!File.Exists(SoundFilePath))
            {
                throw new Exception("sa1.mp3 not found");
            }
        }

        string term = Environment.GetEnvironmentVariable("TERM") ?? "vt100";
        if (OperatingSystem.IsWindows())
        {
            if (CheckSupport())
            {
                term = "windows";
            }
        }

        var isVt = Regex.Match(term, @"vt(\d+)");

        bool enableScreenBuffer = !(isVt.Success || term == "linux");

        bool enableColor = !isVt.Success ||
                           (isVt.Success && int.Parse(Regex.Match(isVt.Value, @"\d+").Value) >= 241);

        int termColumns = 80;
        int termLines = 24;
        if (!isVt.Success)
        {
            try
            {
                termColumns = Console.BufferWidth;
                termLines = Console.BufferHeight;
            }
            catch (Exception)
            {
                // ignore
            }
        }

        string? envCol = Environment.GetEnvironmentVariable("COLUMNS");
        string? envLine = Environment.GetEnvironmentVariable("LINES");
        if (envCol != null && int.TryParse(envCol, out int temp))
        {
            termColumns = temp;
        }

        if (envLine != null && int.TryParse(envLine, out int temp2))
        {
            termLines = temp2;
        }

        if (termColumns < 80 || termLines < 24)
        {
            throw new Exception("the terminal size should be at least 80x24");
        }

        int asciiArtWidth = 40;
        int asciiArtHeight = 20;

        int creditsWidth = Math.Min((termColumns - 4) / 2, 56);
        int creditsHeight = termLines - asciiArtHeight - 2;

        int lyricWidth = termColumns - 4 - creditsWidth;
        int lyricHeight = termLines - 2;

        int asciiArtX = lyricWidth + 4 + (creditsWidth - asciiArtWidth) / 2;
        int asciiArtY = creditsHeight + 3;

        int creditsPosX = lyricWidth + 4;

        AsciiArtHeight = asciiArtHeight;
        AsciiArtX = asciiArtX;
        AsciiArtY = asciiArtY;
        CreditsWidth = creditsWidth;
        CreditsHeight = creditsHeight;
        CreditsPosX = creditsPosX;
        EnableScreenBuffer = enableScreenBuffer;
        EnableColor = enableColor;
        LyricHeight = lyricHeight;
        LyricWidth = lyricWidth;
        IsVt = isVt;
        EnableSound = enableSound;
    }

    private CancellationTokenSource IsEndDraw { get; } = new();

    public int CreditsPosX { get; }
    public int AsciiArtX { get; }
    public int AsciiArtY { get; }
    public int AsciiArtHeight { get; }
    public int LyricWidth { get; }
    public int LyricHeight { get; }
    public int CreditsWidth { get; }
    public int CreditsHeight { get; }
    private Match IsVt { get; }
    private bool EnableScreenBuffer { get; }
    private bool EnableColor { get; }
    public bool EnableSound { get; }

    [SupportedOSPlatform("windows")]
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [SupportedOSPlatform("windows")]
    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [SupportedOSPlatform("windows")]
    private static bool CheckSupport()
    {
        nint handle = GetStdHandle(-11);
        if (GetConsoleMode(handle, out uint mode))
        {
            return (mode & 0x0004) != 0;
        }

        return false;
    }

    public void Run()
    {
        IsEndDraw.Token.ThrowIfCancellationRequested();

        BeginDraw();
        DrawFrame();
        MoveTo(2, 2);
        Thread.Sleep(TimeSpan.FromSeconds(2));

        LyricTyping? typingState = null;
        ArtState? artState = null;
        CreditState? creditState = null;
        int lyricIndex = 0;
        int cursorX = 2;
        int cursorY = 2;

        var stopwatch = Stopwatch.StartNew();
        var lyrics = LyricData.Lyrics;

        while (!IsEndDraw.IsCancellationRequested)
        {
            if (typingState == null && artState == null)
            {
                if (lyricIndex < lyrics.Count)
                {
                    var current = lyrics[lyricIndex];
                    long pastTime = stopwatch.ElapsedMilliseconds / 10;

                    if (pastTime >= current.Time)
                    {
                        string text = current.Words.ToString()!;
                        int wordCount = text.Length > 0 ? text.Length : 1;
                        double interval;

                        if (current.Interval < 0)
                        {
                            var nextLyric = lyrics[lyricIndex + 1];
                            interval = (nextLyric.Time - current.Time) / 100.0 / wordCount;
                        }
                        else
                        {
                            interval = current.Interval / wordCount;
                        }

                        switch (current.Mode)
                        {
                            case 0:
                                typingState = new LyricTyping(text, interval, true, cursorX, cursorY);
                                break;
                            case 1:
                                typingState = new LyricTyping(text, interval, false, cursorX, cursorY);
                                break;
                            case 2:
                                artState = new ArtState((int)current.Words, AsciiArtHeight);
                                break;
                            case 3:
                                ClearLyrics();
                                typingState = null;
                                cursorX = 2;
                                cursorY = 2;
                                break;
                            case 4:
                                if (EnableSound)
                                    Player.Play(SoundFilePath);
                                break;
                            case 5:
                                creditState = new CreditState(
                                    CreditsData.Credits,
                                    CreditsWidth, CreditsHeight, CreditsPosX);
                                break;
                            case 9:
                                goto end;
                        }
                        lyricIndex++;
                    }
                }
            }

            typingState?.Tick();
            if (typingState?.IsDone == true)
            {
                cursorX = typingState.CursorX;
                cursorY = typingState.CursorY;
                typingState = null;
            }

            artState?.Tick(AsciiArtX, AsciiArtY);
            if (artState?.IsDone == true) artState = null;

            while (creditState?.IsReady() == true)
            {
                creditState.Tick();
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(10));
        }

    end:
        return;
    }

    public void Stop()
    {
        IsEndDraw.Token.ThrowIfCancellationRequested();

        IsEndDraw.Cancel();
        IsEndDraw.Dispose();

        if (EnableColor)
        {
            Console.Write("\x1b[0m");
        }

        if (EnableScreenBuffer)
        {
            Console.Write("\x1b[?1049l");
        }
        else
        {
            Console.Write("\x1b[2J");
            MoveTo(1, 1);
        }
    }

    private void BeginDraw()
    {
        if (EnableScreenBuffer)
        {
            Console.Write("\x1b[?1049h");
        }

        if (EnableColor)
        {
            Console.Write("\x1b[33;40;1m");
        }

        Console.Write("\x1b[2J");
    }

    private void DrawFrame()
    {
        MoveTo(1, 1);

        Print(string.Format(" {0}  {1} ", new string('-', LyricWidth), new string('-', CreditsWidth)), !IsVt.Success);
        for (int i = 0; i < CreditsHeight; i++)
        {
            Print(string.Format("|{0}||{1}|", new string(' ', LyricWidth), new string(' ', CreditsWidth)), !IsVt.Success);
        }

        Print(string.Format("|{0}| {1} ", new string(' ', LyricWidth), new string('-', CreditsWidth)), !IsVt.Success);

        for (int i = 0; i < LyricHeight - 1 - CreditsHeight; i++)
        {
            Print(string.Format("|{0}|", new string(' ', LyricWidth)), true);
        }

        Print(string.Format(" {0} ", new string('-', LyricWidth)), false);
    }

    public static void MoveTo(int x, int y)
    {
        Debug.Assert(x >= 0 && y >= 0);
        Console.Write("\x1b[{0};{1}H", y, x);
    }

    private static void Print(string str, bool newLine)
    {
        if (newLine)
        {
            Console.WriteLine(str);
        }
        else
        {
            Console.Write(str);
        }
    }

    private void ClearLyrics()
    {
        for (int y = 2; y < 2 + LyricHeight; y++)
        {
            MoveTo(2, y);
            Console.Write(new string(' ', LyricWidth));
        }
    }
}
