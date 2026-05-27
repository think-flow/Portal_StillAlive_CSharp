using System.Diagnostics;

namespace PortalStillAlive.Core;

public class LyricTyping
{
    public string Text { get; }
    public int CharIndex { get; private set; }
    public int CursorX { get; private set; }
    public int CursorY { get; private set; }
    public bool IsDone => CharIndex >= Text.Length;

    private readonly double _interval;
    private readonly bool _newLine;
    private readonly Stopwatch _stopwatch;

    public LyricTyping(string text, double interval, bool newLine, int cursorX, int cursorY)
    {
        Text = text;
        _interval = interval;
        _newLine = newLine;
        CursorX = cursorX;
        CursorY = cursorY;
        _stopwatch = Stopwatch.StartNew();
    }

    public void Tick()
    {
        double elapsed = _stopwatch.Elapsed.TotalSeconds;
        while (CharIndex < Text.Length)
        {
            if (elapsed < CharIndex * _interval)
                return;

            Stage.MoveTo(CursorX, CursorY);
            Console.Out.Write(Text[CharIndex]);
            CharIndex++;
            CursorX++;
        }

        if (_newLine && IsDone)
        {
            CursorX = 2;
            CursorY++;
        }
    }
}
