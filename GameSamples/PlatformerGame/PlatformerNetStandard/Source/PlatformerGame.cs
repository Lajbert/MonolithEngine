using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonolithEngine;
using System.IO;

namespace ForestPlatformerExample
{
    public class PlatformerGame : MonolithGame
    {

        public static int CoinCount = 0;

        public static bool ANDROID = false;

        private SpriteFont font;

        private KeyboardState prevKeyboardState;

        public static bool Paused = false;
        public static bool WasGameStarted = false;

        private LDTKMap world;

        public static string CurrentScene;

        protected override void Init()
        {

#if DEBUG
            Config.FIXED_UPDATE_FPS = 60;
#else
            Config.FIXED_UPDATE_FPS = 30;
#endif

            Logger.Info("Launching game...");

            font = Content.Load<SpriteFont>("Fonts/DefaultFont");

            Logger.LogToFile = true;

            Logger.Info("Graphics adapter: " + GraphicsAdapter.DefaultAdapter.Description);

            int desktopWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int desktopHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            
            if (ANDROID)
            {
                VideoConfiguration.RESOLUTION_WIDTH = Window.ClientBounds.Width;
                VideoConfiguration.RESOLUTION_HEIGHT = Window.ClientBounds.Height;
            } 
            else
            {
                int gdc = GDC(desktopWidth, desktopHeight);

                if (desktopWidth / gdc == 16 && desktopHeight / gdc == 9)
                {
#if DEBUG
                    if (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width > 1080)
                    {
                        VideoConfiguration.RESOLUTION_WIDTH = 1920;
                        VideoConfiguration.RESOLUTION_HEIGHT = 1080;
                    }
                    else
                    {
                        VideoConfiguration.RESOLUTION_WIDTH = 1280;
                        VideoConfiguration.RESOLUTION_HEIGHT = 720;
                    }
                    VideoConfiguration.FULLSCREEN = false;
#else
                VideoConfiguration.RESOLUTION_WIDTH = desktopWidth;
                VideoConfiguration.RESOLUTION_HEIGHT = desktopHeight;
                VideoConfiguration.FULLSCREEN = true;
#endif
                }
                else
                {
                    VideoConfiguration.RESOLUTION_WIDTH = 1440;
                    VideoConfiguration.RESOLUTION_HEIGHT = 2560;
                    VideoConfiguration.FULLSCREEN = true;
                }
            }


            Logger.Info("Display resolution: " + VideoConfiguration.RESOLUTION_WIDTH +  "x" + VideoConfiguration.RESOLUTION_HEIGHT);
            Logger.Info("Supported display modes: ");
            foreach (DisplayMode displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                Logger.Info("\t"+ displayMode.ToString());
            }

            VideoConfiguration.FRAME_LIMIT = 0;
            VideoConfiguration.VSYNC = false;
        }

        // greatest common divisor using Euclidean algorithm
        private int GDC(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a | b;
        }

        protected override void LoadGameContent()
        {

            foreach (Camera camera in Cameras)
            {
                camera.Limits = new Rectangle(0, 50, 5470, 700);
            }

            Logger.Debug("Loading map from json...");

            MapSerializer mapSerializer = new LDTKJsonMapParser();
            var filePath = Path.Combine(Content.RootDirectory, "Map/level.json");
            using (Stream stream = TitleContainer.OpenStream(filePath))
            {
                world = mapSerializer.Load(stream);
            }

            // UI text generated with: https://fontmeme.com/pixel-fonts/
            // font: KA1
            // base color: 2A2A57
            // selected color: FF0000

            Logger.Debug("Loading assets: HUD texts...");

            Assets.LoadTexture("HUDNewGameBase", "ForestAssets/UI/new_game_base");
            Assets.LoadTexture("HUDNewGameSelected", "ForestAssets/UI/new_game_selected");
            Assets.LoadTexture("HUDSettingsBase", "ForestAssets/UI/settings_base");
            Assets.LoadTexture("HUDSettingsSelected", "ForestAssets/UI/settings_selected");
            Assets.LoadTexture("HUDQuitBase", "ForestAssets/UI/quit_base");
            Assets.LoadTexture("HUDQuitSelected", "ForestAssets/UI/quit_selected");
            Assets.LoadTexture("HUDContinueBase", "ForestAssets/UI/continue_base");
            Assets.LoadTexture("HUDContinueSelected", "ForestAssets/UI/continue_selected");
            Assets.LoadTexture("HUDVideoSettingsBase", "ForestAssets/UI/video_base");
            Assets.LoadTexture("HUDVideoSettingsSelected", "ForestAssets/UI/video_selected");
            Assets.LoadTexture("HUDAudioSettingsBase", "ForestAssets/UI/audio_base");
            Assets.LoadTexture("HUDAudioSettingsSelected", "ForestAssets/UI/audio_selected");
            Assets.LoadTexture("HUDBackBase", "ForestAssets/UI/back_base");
            Assets.LoadTexture("HUDBackSelected", "ForestAssets/UI/back_selected");
            Assets.LoadTexture("HUDResolutionLabel", "ForestAssets/UI/resolution");
            Assets.LoadTexture("HUDFPSLimitLabel", "ForestAssets/UI/fps_limit");
            Assets.LoadTexture("HUDVsyncLabel", "ForestAssets/UI/vsync");
            Assets.LoadTexture("HUDWindowModeLabel", "ForestAssets/UI/window_mode");
            Assets.LoadTexture("HUD30", "ForestAssets/UI/30");
            Assets.LoadTexture("HUD60", "ForestAssets/UI/60");
            Assets.LoadTexture("HUD120", "ForestAssets/UI/120");
            Assets.LoadTexture("HUDUnlimited", "ForestAssets/UI/unlimited");
            Assets.LoadTexture("HUD720p", "ForestAssets/UI/720p");
            Assets.LoadTexture("HUD1080p", "ForestAssets/UI/1080p");
            Assets.LoadTexture("HUD1440p", "ForestAssets/UI/1440p");
            Assets.LoadTexture("HUD4K", "ForestAssets/UI/4k");
            Assets.LoadTexture("HUDOn", "ForestAssets/UI/on");
            Assets.LoadTexture("HUDOff", "ForestAssets/UI/off");
            Assets.LoadTexture("HUDApplyBase", "ForestAssets/UI/apply_base");
            Assets.LoadTexture("HUDApplySelected", "ForestAssets/UI/apply_selected");
            Assets.LoadTexture("HUDCancelBase", "ForestAssets/UI/cancel_base");
            Assets.LoadTexture("HUDCancelSelected", "ForestAssets/UI/cancel_selected");
            Assets.LoadTexture("HUDWindowed", "ForestAssets/UI/windowed");
            Assets.LoadTexture("HUDFullscreen", "ForestAssets/UI/fullscreen");
            Assets.LoadTexture("HUDArrowRightBase", "ForestAssets/UI/arrow_right_base");
            Assets.LoadTexture("HUDArrowRightSelected", "ForestAssets/UI/arrow_right_selected");
            Assets.LoadTexture("HUDArrowLeftBase", "ForestAssets/UI/arrow_right_base", flipHorizontal: true);
            Assets.LoadTexture("HUDArrowLeftSelected", "ForestAssets/UI/arrow_right_selected", flipHorizontal: true);
            Assets.LoadTexture("HUDLoading", "ForestAssets/UI/loading");
            Assets.LoadTexture("Level1Base", "ForestAssets/UI/level_1_base");
            Assets.LoadTexture("Level1Selected", "ForestAssets/UI/level_1_selected");
            Assets.LoadTexture("Level2Base", "ForestAssets/UI/level_2_base");
            Assets.LoadTexture("Level2Selected", "ForestAssets/UI/level_2_selected");
            Assets.LoadTexture("RestartBase", "ForestAssets/UI/restart_base");
            Assets.LoadTexture("RestartSelected", "ForestAssets/UI/restart_selected");
            Assets.LoadTexture("FinishedText", "ForestAssets/UI/finish");
            Assets.LoadTexture("HUDCointCount", "ForestAssets/UI/HUD-coin-count");

            // Entities

            Logger.Debug("Loading assets: entities...");

            Assets.LoadTexture("CarrotMove", "ForestAssets/Characters/Carrot/carrot@move-sheet");
            Assets.LoadTexture("CarrotHurt", "ForestAssets/Characters/Carrot/carrot@hurt-sheet");
            Assets.LoadTexture("CarrotDeath", "ForestAssets/Characters/Carrot/carrot@death-sheet");
            Assets.LoadTexture("CarrotIdle", "ForestAssets/Characters/Carrot/carrot@idle-sheet");

            Assets.LoadTexture("HeroHurt", "ForestAssets/Characters/Hero/main-character@hurt-sheet");
            Assets.LoadTexture("HeroIdle", "ForestAssets/Characters/Hero/main-character@idle-sheet");
            Assets.LoadTexture("HeroIdleWithItem", "ForestAssets/Characters/Hero/main-character@idle-with-item-sheet");
            Assets.LoadTexture("HeroRun", "ForestAssets/Characters/Hero/main-character@run-sheet");
            Assets.LoadTexture("HeroRunWithItem", "ForestAssets/Characters/Hero/main-character@run-with-item-sheet");
            Assets.LoadTexture("HeroJump", "ForestAssets/Characters/Hero/main-character@jump-sheet");
            Assets.LoadTexture("HeroJumpWithItem", "ForestAssets/Characters/Hero/main-character@jump-with-item-sheet");
            Assets.LoadTexture("HeroWallSlide", "ForestAssets/Characters/Hero/main-character@wall-slide-sheet");
            Assets.LoadTexture("HeroDoubleJump", "ForestAssets/Characters/Hero/main-character@double-jump-sheet");
            Assets.LoadTexture("HeroClimb", "ForestAssets/Characters/Hero/main-character@climb-sheet");
            Assets.LoadTexture("HeroAttack", "ForestAssets/Characters/Hero/main-character@attack-sheet");
            Assets.LoadTexture("HeroPickup", "ForestAssets/Characters/Hero/main-character@pick-up-sheet");
            Assets.LoadTexture("HeroSlide", "ForestAssets/Characters/Hero/main-character@slide-sheet");

            Assets.LoadTexture("TrunkAttack", "ForestAssets/Characters/Trunk/Attack (64x32)");
            Assets.LoadTexture("TrunkBulletPieces", "ForestAssets/Characters/Trunk/Bullet Pieces");
            Assets.LoadTexture("TrunkBullet", "ForestAssets/Characters/Trunk/Bullet");
            Assets.LoadTexture("TrunkHit", "ForestAssets/Characters/Trunk/Hit (64x32)");
            Assets.LoadTexture("TrunkIdle", "ForestAssets/Characters/Trunk/Idle (64x32)");
            Assets.LoadTexture("TrunkRun", "ForestAssets/Characters/Trunk/Run (64x32)");

            Assets.LoadTexture("TurtleSpikesIn", "ForestAssets/Characters/SpikedTurtle/Spikes in (44x26)");
            Assets.LoadTexture("TurtleSpikesOut", "ForestAssets/Characters/SpikedTurtle/Spikes out (44x26)");
            Assets.LoadTexture("TurtleHit", "ForestAssets/Characters/SpikedTurtle/Hit (44x26)");
            Assets.LoadTexture("TurtleIdleSpiked", "ForestAssets/Characters/SpikedTurtle/Idle 1 (44x26)");
            Assets.LoadTexture("TurtleIdleNormal", "ForestAssets/Characters/SpikedTurtle/Idle 2 (44x26)");

            Assets.LoadTexture("IceCreamIdle", "IcySkies/Characters/IceCream/ice-cream@idle");
            Assets.LoadTexture("IceCreamDeath", "IcySkies/Characters/IceCream/ice-cream@death");
            Assets.LoadTexture("IceCreamHurt", "IcySkies/Characters/IceCream/ice-cream@hurt");
            Assets.LoadTexture("IceCreamMove", "IcySkies/Characters/IceCream/ice-cream@move");
            Assets.LoadTexture("IceCreamAttack", "IcySkies/Characters/IceCream/ice-cream@attack");
            Assets.LoadTexture("IceCreamProjectileHit", "IcySkies/Characters/IceCream/ice-cream-projectile@hit");
            Assets.LoadTexture("IceCreamProjectileIdle", "IcySkies/Characters/IceCream/ice-cream-projectile@idle");

            Assets.LoadTexture("Rock1Idle", "IcySkies/Characters/Rock/Rock1_Idle (38x34)");
            Assets.LoadTexture("Rock1Run", "IcySkies/Characters/Rock/Rock1_Run (38x34)");
            Assets.LoadTexture("Rock1Hit", "IcySkies/Characters/Rock/Rock1_Hit");
            Assets.LoadTexture("Rock2Idle", "IcySkies/Characters/Rock/Rock2_Idle (32x28)");
            Assets.LoadTexture("Rock2Run", "IcySkies/Characters/Rock/Rock2_Run (32x28)");
            Assets.LoadTexture("Rock2Hit", "IcySkies/Characters/Rock/Rock2_Hit (32x28)");
            Assets.LoadTexture("Rock3Idle", "IcySkies/Characters/Rock/Rock3_Idle (22x18)");
            Assets.LoadTexture("Rock3Run", "IcySkies/Characters/Rock/Rock3_Run (22x18)");
            Assets.LoadTexture("Rock3Hit", "IcySkies/Characters/Rock/Rock3_Hit (22x18)");

            Assets.LoadTexture("GhostAppear", "IcySkies/Characters/Ghost/Appear (44x30)");
            Assets.LoadTexture("GhostDisappear", "IcySkies/Characters/Ghost/Desappear (44x30)");
            Assets.LoadTexture("GhostHit", "IcySkies/Characters/Ghost/Hit (44x30)");
            Assets.LoadTexture("GhostIdle", "IcySkies/Characters/Ghost/Idle (44x30)");

            Logger.Debug("Loading assets: traps and items...");

            // Traps
            Assets.LoadTexture("Saw", "IcySkies/Traps/Saw/saw");

            // Items

            Assets.LoadTexture("CoinPickup", "ForestAssets/Items/coin-pickup");
            Assets.LoadTexture("CoinPickupEffect", "ForestAssets/Items/pickup-effect");
            Assets.LoadTexture("SpringAnim", "ForestAssets/Items/spring_spritesheet");

            Assets.LoadTexture("ForestTileset", "ForestAssets/Tiles/forest-tileset");

            Assets.LoadTexture("BoxIdle", "ForestAssets/Items/box-idle");
            Assets.LoadTexture("BoxHit", "ForestAssets/Items/box-hit");
            Assets.LoadTexture("BoxDestroy", "ForestAssets/Items/box-destroy");

            Assets.LoadTexture("FanAnim", "IcySkies/Items/Fan/fan");

            Assets.LoadTexture("FinishedTrophy", "IcySkies/Items/POI/End (Idle)");

            Logger.Debug("Loading assets: fonts...");
            Assets.AddFont("InGameText", Content.Load<SpriteFont>("Text/InGameText"));

            // Sounds

            Logger.Debug("Loading assets: sounds...");

            AudioEngine.AddSound("Level1Music", "ForestAssets/Audio/POL-chubby-cat-long", true, AudioTag.MUSIC); 
            AudioEngine.AddSound("Level2Music", "IcySkies/Audio/level_2_bg_2", true, AudioTag.MUSIC);
            AudioEngine.AddSound("HeroPunch", "ForestAssets/Audio/hero_punch");
            AudioEngine.AddSound("SpringBounceSound", "ForestAssets/Audio/spring");
            AudioEngine.AddSound("JumpSound", "ForestAssets/Audio/jump2", maxVolume: 0.5f);
            AudioEngine.AddSound("BoxBounceSound", "ForestAssets/Audio/box_bounce");
            AudioEngine.AddSound("CoinPickupSound", "ForestAssets/Audio/coin_pickup");
            AudioEngine.AddSound("HeroHurtSound", "ForestAssets/Audio/hero_hurt");
            AudioEngine.AddSound("CarrotJumpHurtSound", "ForestAssets/Audio/carrot_jump_hurt");
            AudioEngine.AddSound("BoxExplosionSound", "ForestAssets/Audio/box_explosion");
            AudioEngine.AddSound("FastFootstepsSound", "ForestAssets/Audio/footsteps");
            AudioEngine.AddSound("SlowFootstepsSound", "ForestAssets/Audio/footsteps_slow", true);
            AudioEngine.AddSound("CarrotExplodeSound", "ForestAssets/Audio/carrot_explode");
            AudioEngine.AddSound("MenuHover", "ForestAssets/Audio/menu_hover");
            AudioEngine.AddSound("MenuSelect", "ForestAssets/Audio/menu_select");
            AudioEngine.AddSound("TrunkHit", "ForestAssets/Audio/trunk_damage");
            AudioEngine.AddSound("TrunkShoot", "ForestAssets/Audio/trunk_shoot");
            AudioEngine.AddSound("TrunkDeath", "ForestAssets/Audio/trunk_death");
            AudioEngine.AddSound("LadderClimb", "ForestAssets/Audio/sfx_movement_ladder5loop", true);
            AudioEngine.AddSound("BoxPickup", "ForestAssets/Audio/sfx_sounds_interaction19");
            AudioEngine.AddSound("IceCreamExplode", "ForestAssets/Audio/sfx_weapon_singleshot3"); 
            AudioEngine.AddSound("GostDisappear", "ForestAssets/Audio/sfx_wpn_laser11");
            AudioEngine.AddSound("GostAppear", "ForestAssets/Audio/sfx_wpn_laser11"); 
            AudioEngine.AddSound("RockSplit", "ForestAssets/Audio/sfx_sounds_impact10");

            if (ANDROID)
            {
                Assets.LoadTexture("LeftArrow", "MobileButtons/left_arrow");
                Assets.LoadTexture("RightArrow", "MobileButtons/right_arrow");
                Assets.LoadTexture("UpArrow", "MobileButtons/up_arrow");
                Assets.LoadTexture("DownArrow", "MobileButtons/down_arrow");
                Assets.LoadTexture("XButton", "MobileButtons/x_button");
                Assets.LoadTexture("SquareButton", "MobileButtons/square_button");
                Assets.LoadTexture("CircleButton", "MobileButtons/circle_button");
                Assets.LoadTexture("TriangleButton", "MobileButtons/triangle_button");
                Assets.LoadTexture("MenuButton", "MobileButtons/menu_button");
            }

#if DEBUG
            AudioEngine.MuteAll();
#endif

            Logger.Debug("Loading scenes...");

            MainMenuScene mainMenuScene = new MainMenuScene();
            PauseMenuScene pauseMenuScene = new PauseMenuScene();
            Level1Scene level1 = new Level1Scene(world, font);
            VideoSettingsScene videoSettings = new VideoSettingsScene();
            LoadingScreenScene loadingScreen = new LoadingScreenScene();
            Level2Scene level2 = new Level2Scene(world, font);
            LevelSelectScreen levelSelectScreen = new LevelSelectScreen();
            GameEndScene endScene = new GameEndScene();
            if (!ANDROID)
            {
                SettingsScene settings = new SettingsScene();
                SceneManager.AddScene(settings);
            }

            world = null;

            SceneManager.AddScene(mainMenuScene);
            SceneManager.AddScene(pauseMenuScene);
            SceneManager.AddScene(level1);
            SceneManager.AddScene(videoSettings);
            SceneManager.AddScene(loadingScreen);
            SceneManager.AddScene(level2);
            SceneManager.AddScene(levelSelectScreen);
            SceneManager.AddScene(endScene);


            Logger.Debug("Starting main menu...");
            SceneManager.SetLoadingScene(loadingScreen);
            SceneManager.LoadScene(mainMenuScene);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            KeyboardState state = Keyboard.GetState();

            if (prevKeyboardState != state && state.IsKeyDown(Keys.R))
            {
                SceneManager.LoadScene(CurrentScene);
            }
            else if (prevKeyboardState != state && state.IsKeyDown(Keys.Escape) && WasGameStarted && !Paused)
            {
                SceneManager.StartScene("PauseMenu");
            }
            else if (prevKeyboardState != state && state.IsKeyDown(Keys.Escape) && WasGameStarted && Paused)
            {
                SceneManager.StartScene(CurrentScene);
            } 
            else if (prevKeyboardState != state && state.IsKeyDown(Keys.Escape) && !WasGameStarted)
            {
                SceneManager.StartScene("MainMenu");
            }

            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            prevKeyboardState = state;
        }
    }
}
