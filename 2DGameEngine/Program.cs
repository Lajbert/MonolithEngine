﻿using System;
using GameEngine2D.GameExamples2D.SideScroller.src;
namespace GameEngine2D
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new SideScrollerMain())
                game.Run();
        }
    }
}
