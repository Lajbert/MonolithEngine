﻿using System;
using ForestPlatformerExample;
using Foundation;
using MonolithEngine;
using UIKit;

namespace PlatformerIOS
{
    [Register("AppDelegate")]
    class Program : UIApplicationDelegate
    {
        private static PlatformerGame game;

        internal static void RunGame()
        {
            PlatformerGame.ANDROID = true;
            Config.ANDROID = true;
            game = new PlatformerGame();
            game.Run();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            UIApplication.Main(args, null, "AppDelegate");
        }

        public override void FinishedLaunching(UIApplication app)
        {
            RunGame();
        }
    }
}
