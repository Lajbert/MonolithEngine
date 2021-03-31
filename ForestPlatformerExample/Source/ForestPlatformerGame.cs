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

        public static bool GameRunning = false;
        public static bool Paused = false;

        protected override void Init()
        {
            font = Content.Load<SpriteFont>("DefaultFont");
        }

        protected override void LoadGameContent()
        {

            Assets.LoadTexture("CoinPickup", "ForestAssets/Items/coin-pickup");
            Assets.LoadTexture("CoinPickupEffect", "ForestAssets/Items/pickup-effect");
            Assets.LoadTexture("SpringAnim", "ForestAssets/Items/spring_spritesheet");

            Assets.LoadTexture("ForestTileset", "ForestAssets/Tiles/forest-tileset");

            Assets.LoadTexture("BoxIdle", "ForestAssets/Items/box-idle");
            Assets.LoadTexture("BoxHit", "ForestAssets/Items/box-hit");
            Assets.LoadTexture("BoxDestroy", "ForestAssets/Items/box-destroy");

            Assets.LoadTexture("HUDCointCount", "ForestAssets/UI/HUD-coin-count");

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

            MainMenuScene mainMenuScene = new MainMenuScene(Camera);
            PauseMenuScene pauseMenuScene = new PauseMenuScene(Camera);
            Level1Scene level1 = new Level1Scene(Camera, font);

            SceneManager.AddScene(mainMenuScene);
            SceneManager.AddScene(pauseMenuScene);
            SceneManager.AddScene(level1);

            SceneManager.LoadScene(mainMenuScene);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            KeyboardState state = Keyboard.GetState();

            if (prevKeyboardState != state && state.IsKeyDown(Keys.R) /*GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||*/)
            {
                SceneManager.LoadScene("Level1");
            }

            else if (prevKeyboardState != state && state.IsKeyDown(Keys.Escape) && GameRunning /*GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||*/)
            {
                SceneManager.StartScene("PauseMenu");
            }

            else if (prevKeyboardState != state && state.IsKeyDown(Keys.Escape) && Paused /*GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||*/)
            {
                SceneManager.StartScene("Level1");
            }

            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            prevKeyboardState = state;
        }
    }
}
