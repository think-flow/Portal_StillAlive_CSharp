using PortalStillAlive.Data;

namespace PortalStillAlive.Core;

public class ArtState
{
    public int Ch { get; }
    public int Dy { get; private set; }
    public bool IsDone => Dy >= _height;

    private readonly int _height;

    public ArtState(int ch, int height)
    {
        Ch = ch;
        _height = height;
    }

    public void Tick(int artX, int artY)
    {
        if (IsDone) return;

        Stage.MoveTo(artX, artY + Dy);
        Console.Write(AsciiArtsData.Arts[Ch][Dy]);
        Dy++;
    }
}
