using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Global
{
    public class Config
    {
        public static float CHARACTER_SPEED = 1f;
        public static float ZOOM = 1f;
        public static int   GRID = 16;
        public static float FRICTION = 0.2f;
        public static float BUMB_FRICTION = 0.5f;
        //public static float BUMB_FRICTION = 0f;
        public static bool  GRAVITY_ON = true;
        public static float GRAVITY_FORCE = 8f;
        public static float JUMP_FORCE = 10f;
        public static float GRAVITY_T_MULTIPLIER = 0.5f;

        public static float SPRITE_DRAW_OFFSET = 0f;
        public static float SPRITE_COLLISION_OFFSET = 0f;

        public static float TIME_OFFSET = 10;
        public static int RES_W = 1920;
        public static int RES_H = 1080;
        public static int FPS = 60;
        public static bool FULLSCREEN = false;
        public static int PIVOT_RADIUS = 10;

        public static int CAMERA_TIME_MULTIPLIER = 1; // set this and CAMERA_FOLLOW_DELAY to a higher value to create a "wabbly" camera
        public static float CAMERA_DEADZONE = 100;
        public static float CAMERA_FRICTION = 0.89f;
        public static float CAMERA_FOLLOW_DELAY = 0.0005f;
        public static float CAMERA_ZOOM = 1;
    }
}
