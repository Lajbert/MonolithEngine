using GameEngine2D.GameExamples2D.SideScroller.src;
using System;

namespace SideScrollerExample
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new SideScrollerGame())
                game.Run();
        }
    }
}
