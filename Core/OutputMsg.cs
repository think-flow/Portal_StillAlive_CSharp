using System.Diagnostics;
using System.Threading.Channels;

namespace PortalStillAlive.Core;

public struct OutputMsg
{
    public Position Position;
    public string Content;
}

public struct Position
{
    public ushort X;
    public ushort Y;

    public void Deconstruct(out ushort x, out ushort y)
    {
        x = X;
        y = Y;
    }
}

public static class ChannelWriterEx
{
    public static void Print(this ChannelWriter<OutputMsg> tx, (int, int) position, string content)
    {
        (int x, int y) = position;
        Debug.Assert(x >= 0 && y >= 0);

        tx.TryWrite(new OutputMsg
        {
            Position = new Position {X = (ushort) x, Y = (ushort) y},
            Content = content
        });
    }
}
