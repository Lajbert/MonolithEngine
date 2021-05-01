using System;

namespace ForestPlatformerExample
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new PlatformerGame())
                game.Run();
        }
    }
}
