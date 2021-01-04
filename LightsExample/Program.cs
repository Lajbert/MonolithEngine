using GameEngine2D.GameExamples2D.SideScroller.src;
using System;

namespace LightsExample
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new LightsDemo())
                game.Run();
        }
    }
}
