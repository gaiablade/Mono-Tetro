using System;

namespace BinksFarm;

public static class Program
{
    [STAThread]
    static void Main()
    {
        using (var game = new App())
            game.Run();
    }
}
