using GameEngine2D.GameExamples.TopDown.src;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Global
{
    class Config
    {
        public static readonly float CHARACTER_SPEED = 1f;
        public static readonly int GRID = 64;
        public static readonly float FRICTION = 0.2f;
        public static readonly float BUMB_FRICTION = 0.5f;
        public static bool GRAVITY_ON = true;
        public static readonly float GRAVITY_FORCE = 8f;
        public static readonly float JUMP_FORCE = 10f;
        public static readonly float GRAVITY_T_MULTIPLIER = 1.5f;

        public static readonly float SPRITE_DRAW_OFFSET = 0f;
        public static readonly float SPRITE_COLLISION_OFFSET = 0f;

        public static readonly float TIME_OFFSET = 10;
        public static readonly int RES_W = 1920;
        public static readonly int RES_H = 1080;
        public static readonly int FPS = 60;
        public static readonly bool FULLSCREEN = false;
        public static readonly int PIVOT_RADIUS = 10;

        public static readonly int CAMERA_TIME_MULTIPLIER = 1; // set this and CAMERA_FOLLOW_DELAY to a higher value to create a "wabbly" camera
        public static readonly int CAMERA_DEADZONE = 250;
        public static readonly float CAMERA_FRICTION = 0.89f;
        public static readonly float CAMERA_FOLLOW_DELAY = 0.0005f;
    }
}
