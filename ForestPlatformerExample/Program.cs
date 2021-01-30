using System;
using TestExample;

namespace ForestPlatformerExample
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new ForestPlatformer())
                game.Run();
        }
    }
}
