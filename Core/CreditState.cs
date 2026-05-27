using System.Diagnostics;

namespace PortalStillAlive.Core;

public class CreditState
{
    private readonly string _credits;
    private readonly int _creditsWidth;
    private readonly int _creditsHeight;
    private readonly int _creditsPosX;
    private readonly Stopwatch _stopwatch;

    private int _j;
    private int _charPos;
    private readonly List<int> _lineStarts = [0];

    public bool IsDone => _j >= _credits.Length;

    public CreditState(string credits, int creditsWidth, int creditsHeight, int creditsPosX)
    {
        _credits = credits;
        _creditsWidth = creditsWidth;
        _creditsHeight = creditsHeight;
        _creditsPosX = creditsPosX;
        _stopwatch = Stopwatch.StartNew();
    }

    public bool IsReady()
    {
        if (IsDone) return false;
        return _stopwatch.Elapsed.TotalSeconds >= 174.0 * _j / _credits.Length;
    }

    public void Tick()
    {
        if (IsDone) return;

        char ch = _credits[_j];
        _j++;

        if (ch == '\n')
        {
            _lineStarts.Add(_j);
            _charPos = 0;
            RedrawVisible();
        }
        else
        {
            Stage.MoveTo(_creditsPosX + _charPos, _creditsHeight + 1);
            Console.Write(ch);
            _charPos++;
        }
    }

    private void RedrawVisible()
    {
        int visibleStart = Math.Max(0, _lineStarts.Count - _creditsHeight);
        for (int i = visibleStart; i < _lineStarts.Count; i++)
        {
            int y = 2 + _creditsHeight - _lineStarts.Count + i;
            Stage.MoveTo(_creditsPosX, y);

            int lineEnd = (i + 1 < _lineStarts.Count)
                ? _lineStarts[i + 1] - 1
                : _j - 1;
            lineEnd = Math.Max(_lineStarts[i], lineEnd);
            string line = _credits[_lineStarts[i]..lineEnd];
            Console.Write(line.PadRight(_creditsWidth));
        }
    }
}
