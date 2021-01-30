using GameEngine2D.Engine.Source.Entities.Animations;
using GameEngine2D.Engine.Source.Util;
using GameEngine2D.Entities;
using GameEngine2D.Source.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Items
{
    class Coin : Entity
    {
        public Coin(Vector2 position) : base(LayerManager.Instance.EntityLayer, null, position, null)
        {

            Active = true;

            ColliderOnGrid = true;

            //DEBUG_SHOW_PIVOT = true;

            Animations = new AnimationStateMachine();
            Animations.Offset = new Vector2(4, 3);

            Texture2D spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Items-and-Objects/Sprite-Sheets/coin-pickup");
            SpriteSheetAnimation coinAnim = new SpriteSheetAnimation(this, spriteSheet, 5, 5, 24, 32, 32, 24);
            Animations.RegisterAnimation("Idle", coinAnim);

            spriteSheet = SpriteUtil.LoadTexture("Green_Greens_Forest_Pixel_Art_Platformer_Pack/Items-and-Objects/Sprite-Sheets/pickup-effect");
            SpriteSheetAnimation pickupAnim = new SpriteSheetAnimation(this, spriteSheet, 2, 2, 6, 32, 32, 24);
            SetDestroyAnimation(pickupAnim);

            //SetSprite(SpriteUtil.CreateRectangle(16, Color.Black));
            //Pivot = new Vector2(5, 5);
        }
    }
}
