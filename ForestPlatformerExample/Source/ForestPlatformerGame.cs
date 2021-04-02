using ForestPlatformerExample.Source.Enemies;
using ForestPlatformerExample.Source.Entities.Items;
using ForestPlatformerExample.Source.Environment;
using ForestPlatformerExample.Source.Items;
using ForestPlatformerExample.Source.PlayerCharacter;
using ForestPlatformerExample.Source.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonolithEngine;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Entities;
using MonolithEngine.Engine.Source.Level;
using MonolithEngine.Engine.Source.MyGame;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.UI;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Entities;
using MonolithEngine.Global;
using MonolithEngine.Source.Level;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source
{
    class ForestPlatformerGame : MonolithGame
    {

        public static int CoinCount = 0;

        private SpriteFont font;

        private KeyboardState prevKeyboardState;

        public static bool Paused = false;
        public static bool WasGameStarted = false;

        protected override void Init()
        {
            font = Content.Load<SpriteFont>("DefaultFont");


            //graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            VideoConfiguration.RESOLUTION_WIDTH = 2560;
            VideoConfiguration.RESOLUTION_HEIGHT = 1440;
            VideoConfiguration.FULLSCREEN = false;
            VideoConfiguration.FRAME_LIMIT = 0;
            VideoConfiguration.VSYNC = true;
        }

        protected override void LoadGameContent()
        {
            // UI text generated with: https://fontmeme.com/pixel-fonts/
            // font: KA1
            // base color: 2A2A57
            // selected color: FF0000
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

            // Entities

            Assets.LoadTexture("CoinPickup", "ForestAssets/Items/coin-pickup");
            Assets.LoadTexture("CoinPickupEffect", "ForestAssets/Items/pickup-effect");
            Assets.LoadTexture("SpringAnim", "ForestAssets/Items/spring_spritesheet");

            Assets.LoadTexture("ForestTileset", "ForestAssets/Tiles/forest-tileset");

            Assets.LoadTexture("BoxIdle", "ForestAssets/Items/box-idle");
            Assets.LoadTexture("BoxHit", "ForestAssets/Items/box-hit");
            Assets.LoadTexture("BoxDestroy", "ForestAssets/Items/box-destroy");

            Assets.LoadTexture("HUDCointCount", "ForestAssets/UI/HUD-coin-count");

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

            MainMenuScene mainMenuScene = new MainMenuScene();
            PauseMenuScene pauseMenuScene = new PauseMenuScene();
            Level1Scene level1 = new Level1Scene(font);
            SettingsScene settings = new SettingsScene();
            VideoSettingsScene videoSettings = new VideoSettingsScene();
            LoadingScreenScene loadingScreen = new LoadingScreenScene();

            SceneManager.AddScene(mainMenuScene);
            SceneManager.AddScene(settings);
            SceneManager.AddScene(pauseMenuScene);
            SceneManager.AddScene(level1);
            SceneManager.AddScene(videoSettings);
            SceneManager.AddScene(loadingScreen);

            SceneManager.LoadScene(mainMenuScene);
            SceneManager.SetLoadingScene(loadingScreen);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            KeyboardState state = Keyboard.GetState();

            if (prevKeyboardState != state && state.IsKeyDown(Keys.R))
            {
                SceneManager.LoadScene("Level1");
            }
            else if (prevKeyboardState != state && state.IsKeyDown(Keys.Escape) && WasGameStarted && !Paused)
            {
                SceneManager.StartScene("PauseMenu");
            }
            else if (prevKeyboardState != state && state.IsKeyDown(Keys.Escape) && WasGameStarted && Paused)
            {
                SceneManager.StartScene("Level1");
            } else if (prevKeyboardState != state && state.IsKeyDown(Keys.Escape) && !WasGameStarted)
            {
                SceneManager.StartScene("MainMenu");
            }

            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            prevKeyboardState = state;
        }
    }
}
