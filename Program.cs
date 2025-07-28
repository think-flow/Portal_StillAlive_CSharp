using PortalStillAlive.Core;

namespace PortalStillAlive;

internal class Program
{
    private static void Main(string[] _)
    {
        Stage? stage = null;
        try
        {
            stage = new Stage();

            Console.CancelKeyPress += (_, _) =>
            {
                stage.Stop();
                Console.WriteLine("Got it! Exiting...");
                Environment.Exit(1);
            };

            stage.Run();
            stage.Stop();
        }
        catch (Exception e)
        {
            stage?.Stop();
            Console.Error.WriteLine(e.Message);
            Environment.Exit(2);
        }
    }
}
