using System;
using ForestPlatformerExample;
using MonolithEngine;

namespace PlatformerOpenGL
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (PlatformerGame game = new PlatformerGame())
            {
                game.Run();
            }
        }
    }
}
