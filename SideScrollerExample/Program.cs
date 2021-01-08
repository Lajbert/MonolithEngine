using SideScrollerExample.SideScroller;
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
