using GameEngine2D.Engine.src.Entities;
using GameEngine2D.Engine.src.Util;
using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.src;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SideScrollerExample.SideScroller.src.Entities
{
    class Bullet : Entity
    {
        private float speed = 50f;
        private int mul = 1;
        public Bullet(Entity parent, FaceDirection faceDirection) : base(Scene.Instance.EntityLayer, null, parent.Position)
        {
            Logger.Log("Parent position: " + parent.Position);
            Logger.Log("Parent position with root: " + parent.GetPositionWithParent());
            if (faceDirection == FaceDirection.LEFT)
            {
                mul = -1;
            }

            SetSprite(SpriteUtil.CreateRectangle(GraphicsDeviceManager, Config.GRID / 3, Color.Red));
        }

        public override void Update(GameTime gameTime)
        {
            X += speed * mul;

            base.Update(gameTime);
        }
    }
}
