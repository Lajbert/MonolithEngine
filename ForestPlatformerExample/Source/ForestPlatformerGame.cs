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

        protected override void Init()
        {
            font = Content.Load<SpriteFont>("DefaultFont");
        }

        protected override void LoadGameContent()
        {
            Level1Scene level1 = new Level1Scene(Camera, font);
            SceneManager.AddScene(level1);

            SceneManager.LoadScene(level1);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.R))
            {
                SceneManager.LoadScene("level1");
            }
        }
    }
}
