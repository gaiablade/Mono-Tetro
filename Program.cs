using System;

namespace Monomino;

public static class Program
{
    [STAThread]
    static void Main()
    {
        try
        {
            using (var game = new App())
                game.Run();
        }
        catch (Exception e)
        {
            LogManager.Log($"FATAL ERROR: {e}");
        }
    }
}
