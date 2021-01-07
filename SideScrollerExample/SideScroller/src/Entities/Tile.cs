using GameEngine2D.Engine.src.Entities.Animations;
using GameEngine2D.Engine.src.Layer;
using GameEngine2D.Engine.src.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.src;
using GameEngine2D.src.Entities.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace SideScrollerExample.SideScroller.src.Entities
{

    public class Tile : Entity
    {

        private float scale = 2.5f;
        private Rectangle sourceRectangle = new Rectangle(0, 0, 63, 63);
        private Vector2 spriteOffset = new Vector2(-50f, -50f);
        private int animationFps = 120;

        public Tile(ContentManager contentManager, AbstractLayer layer, Vector2 position, Color color, SpriteFont font = null) : base(layer, null, position, null, font)
        {
            Sprite = SpriteUtil.CreateRectangle(Config.GRID, color);
            Animations = new AnimationStateMachine();
            //Animations = new AnimationStateMachine();
            string folder = "PixelSimulations/";
            List<Texture2D> explosion = SpriteUtil.LoadTextures(folder + "Explosion3/000", 1, 9, contentManager);
            explosion.AddRange(SpriteUtil.LoadTextures(folder + "Explosion3/00", 10, 30, contentManager));

            AnimatedSpriteGroup explode = new AnimatedSpriteGroup(explosion, this, SpriteBatch, animationFps);
            explode.Scale = scale;
            Animations.Offset = spriteOffset;
            SetDestroyAnimation(explode);

            //DestroySound = contentManager.Load<SoundEffect>("Audio/Effects/Explosion");
            //Func<bool> isExploding = () => true;
            //Animations.RegisterAnimation("Idle", explode, isExploding);
        }
    }
}
