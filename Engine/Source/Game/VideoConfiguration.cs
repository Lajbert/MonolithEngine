namespace MonolithEngine
{
    public class VideoConfiguration
    {
        internal static MonolithGame GameInstance;

        public static int RESOLUTION_WIDTH = 0;
        public static int RESOLUTION_HEIGHT = 0;
        public static bool VSYNC = true;
        public static int FRAME_LIMIT = 0;
        public static bool FULLSCREEN = true;

        public static void Apply()
        {
            GameInstance.ApplyVideoConfiguration();
        }
    }
}
