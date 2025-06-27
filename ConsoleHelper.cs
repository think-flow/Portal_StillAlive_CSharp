using System.Text.RegularExpressions;
using PortalStillAlive.Data;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace PortalStillAlive;

public static class ConsoleHelper
{
    private static readonly Lock _lock = new();
    private static readonly bool _enableScreenBuffer;
    private static readonly bool _enableColor;
    private static readonly Match _isVt;
    private static bool _isDrawEnd;

    private static int _cursorX;
    private static int _cursorY;

    private static readonly int _termColumns;
    private static readonly int _termLines;

    private static readonly int _lyricWidth;
    private static readonly int _lyricHeight;

    private static readonly int _creditsWidth;
    private static readonly int _creditsHeight;

    private static readonly int _asciiArtWidth;
    private static readonly int _asciiArtHeight;

    private static readonly int _asciiArtX;
    private static readonly int _asciiArtY;

    private static readonly int _creditsPosX;

    static ConsoleHelper()
    {
        string term = Environment.GetEnvironmentVariable("TERM") ?? "vt100";
        _isVt = Regex.Match(term, @"vt(\d+)");

        // xterm, rxvt, konsole ...
        // but fbcon in linux kernel does not support screen buffer
        _enableScreenBuffer = !(_isVt.Success || term == "linux");

        // color support is after VT241
        _enableColor = !_isVt.Success ||
                       (_isVt.Success && int.Parse(Regex.Match(_isVt.Value, @"\d+").Value) >= 241);

        if (_isVt.Success)
        {
            _termColumns = 80;
            _termLines = 24;
        }
        else
        {
            _termColumns = Console.BufferWidth;
            _termLines = Console.BufferHeight;
        }

        string? envCol = Environment.GetEnvironmentVariable("COLUMNS");
        string? envLine = Environment.GetEnvironmentVariable("LINES");
        if (envCol != null && int.TryParse(envCol, out int temp))
        {
            _termColumns = temp;
        }

        if (envLine != null && int.TryParse(envLine, out int temp2))
        {
            _termLines = temp2;
        }

        if (_termColumns < 80 || _termLines < 24)
        {
            throw new Exception("the terminal size should be at least 80x24");
        }

        _asciiArtWidth = 40;
        _asciiArtHeight = 20;

        _creditsWidth = Math.Min((_termColumns - 4) / 2, 56);
        _creditsHeight = _termLines - _asciiArtHeight - 2;

        _lyricWidth = _termColumns - 4 - _creditsWidth;
        _lyricHeight = _termLines - 2;

        _asciiArtX = _lyricWidth + 4 + (_creditsWidth - _asciiArtWidth) / 2;
        _asciiArtY = _creditsHeight + 3;

        _creditsPosX = _lyricWidth + 4;
    }

    public static void BeginDraw()
    {
        if (_enableScreenBuffer)
        {
            lock (_lock)
            {
                Console.Write("\x1b[?1049h");
            }
        }

        if (_enableColor)
        {
            lock (_lock)
            {
                Console.Write("\x1b[33;40;1m");
            }
        }
    }

    public static void EndDraw()
    {
        lock (_lock)
        {
            _isDrawEnd = true;
            if (_enableColor)
            {
                Console.Write("\x1b[0m");
            }

            if (_enableScreenBuffer)
            {
                Console.Write("\x1b[?1049l");
            }
            else
            {
                Clear(false);
                Move(1, 1, false, false);
            }
        }
    }

    public static void DrawFrame()
    {
        Move(1, 1);
        Print(" " + new string('-', _lyricWidth) + "  " + new string('-', _creditsWidth) + " ", !_isVt.Success);
        for (int i = 0; i < _creditsHeight; i++)
        {
            Print("|" + new string(' ', _lyricWidth) + "||" + new string(' ', _creditsWidth) + "|", !_isVt.Success);
        }

        Print("|" + new string(' ', _lyricWidth) + "| " + new string('-', _creditsWidth) + " ", !_isVt.Success);

        for (int i = 0; i < _lyricHeight - 1 - _creditsHeight; i++)
        {
            Print("|" + new string(' ', _lyricWidth) + "|");
        }

        Print(" " + new string('-', _lyricWidth) + " ", false);
        Move(2, 2);
        Console.Out.Flush();
        Thread.Sleep(1000);
    }

    public static void Move(int x, int y, bool updateCursor = true, bool mutex = true)
    {
        if (mutex)
        {
            lock (_lock)
            {
                Console.Write("\x1b[{0};{1}H", y, x);
                Console.Out.Flush();
                if (updateCursor)
                {
                    _cursorX = x;
                    _cursorY = y;
                }
            }
        }
        else
        {
            Console.Write("\x1b[{0};{1}H", y, x);
            Console.Out.Flush();
            if (updateCursor)
            {
                _cursorX = x;
                _cursorY = y;
            }
        }
    }

    public static void Clear(bool mutex = true)
    {
        _cursorX = 1;
        _cursorY = 1;
        if (mutex)
        {
            lock (_lock)
            {
                Console.Write("\x1b[2J");
            }
        }
        else
        {
            Console.Write("\x1b[2J");
        }
    }

    private static void Print(string str, bool newLine = true)
    {
        lock (_lock)
        {
            if (newLine)
            {
                Console.WriteLine(str);
                _cursorX = 1;
                _cursorY += 1;
            }
            else
            {
                Console.Write(str);
                _cursorX += str.Length;
            }
        }
    }

    public static int DrawLyrics(string str, int x, int y, double interval, bool newLine)
    {
        Move(x + 2, y + 2);
        foreach (char c in str)
        {
            Print(c.ToString(), false);
            Console.Out.Flush();
            Thread.Sleep(TimeSpan.FromSeconds(interval));
            x += 1;
        }

        if (newLine)
        {
            x = 0;
            y += 1;
            Move(2, y + 2);
        }

        return x;
    }

    public static void DrawAsciiArts(int ch)
    {
        string[][] arts = AsciiArtsData.Arts;
        for (int dy = 0; dy < _asciiArtHeight; dy++)
        {
            Move(_asciiArtX, _asciiArtY + dy);
            Console.Write(arts[ch][dy]);
            Console.Out.Flush();
            Thread.Sleep(10);
        }
    }

    public static void ClearLyrics()
    {
        Move(1, 2);
        for (int i = 0; i < _lyricHeight; i++)
        {
            Print("|" + new string(' ', _lyricWidth));
        }

        Move(2, 2);
    }

    public static void DrawCredits()
    {
        var thread = new Thread(Start)
        {
            IsBackground = true
        };
        thread.Start();

        void Start()
        {
            string credits = CreditsData.Credits;
            int creditX = 0;
            int i = 0;
            int length = credits.Length;
            var lastCredits = new List<string> {""};
            double startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d;

            foreach (char ch in credits)
            {
                double currentTime = startTime + 174d / length * i;
                i += 1;
                if (ch == '\n')
                {
                    creditX = 0;
                    lastCredits.Add("");
                    if (lastCredits.Count > _creditsHeight)
                    {
                        lastCredits = lastCredits.TakeLast(_creditsHeight).ToList();
                    }

                    lock (_lock)
                    {
                        if (_isDrawEnd)
                        {
                            break;
                        }

                        for (int y = 2; y < 2 + _creditsHeight - lastCredits.Count; y++)
                        {
                            Move(_creditsPosX, y, false, false);
                            Console.Write(new string(' ', _creditsWidth));
                        }

                        for (int k = 0; k < lastCredits.Count; k++)
                        {
                            int y = 2 + _creditsHeight - lastCredits.Count + k;
                            Move(_creditsPosX, y, false, false);
                            Console.Write(lastCredits[k]);
                            Console.Write(new string(' ', _creditsWidth - lastCredits[k].Length));
                        }

                        Move(_cursorX, _cursorY, false, false);
                    }
                }
                else
                {
                    lastCredits[^1] += ch;
                    lock (_lock)
                    {
                        if (_isDrawEnd)
                        {
                            break;
                        }

                        Move(_creditsPosX + creditX, _creditsHeight + 1, false, false);
                        Console.Write(ch.ToString());
                        Move(_cursorX, _cursorY, false, false);
                    }

                    creditX += 1;
                }

                while (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d < currentTime)
                {
                    Thread.Sleep(10);
                }
            }
        }
    }
}
