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
        private float speed = 15f;
        private int mul = 1;

        public Bullet(Entity parent, FaceDirection faceDirection) : base(Scene.Instance.EntityLayer, null, parent.Position)
        {
            if (faceDirection == GameEngine2D.Engine.src.Entities.FaceDirection.LEFT)
            {
                mul = -1;
            }

            SinglePointCollisionChecks.Add(faceDirection);

            SetSprite(SpriteUtil.CreateRectangle(GraphicsDeviceManager, Config.GRID / 3, Color.Red));

            Logger.Log("Bullet created");
        }

        public override void Update(GameTime gameTime)
        {
            X += speed * mul;

            base.Update(gameTime);
        }

        protected override void OnCollisionStart(Entity otherCollider)
        {
            otherCollider.Destroy();
            Destroy();
        }

        protected override void OnCollisionEnd(Entity otherCollider)
        {

        }
    }
}
