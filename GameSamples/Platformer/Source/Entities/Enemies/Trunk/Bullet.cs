using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonolithEngine;

namespace ForestPlatformerExample
{
    class Bullet : PhysicalEntity
    {

        public Bullet(AbstractScene scene, Vector2 position, Vector2 speed) : base(scene.LayerManager.EntityLayer, null, position)
        {
            DrawPriority = 3;

            AddTag("Projectile");
            Sprite sprite = new Sprite(this, Assets.GetTexture("TrunkBullet"));
            if (speed.X > 0)
            {
                sprite.SpriteEffect = SpriteEffects.FlipHorizontally;
            }
            AddComponent(sprite);

            CircleCollisionComponent collider = new CircleCollisionComponent(this, 3, new Vector2(8, 8));
            AddComponent(collider);
            CheckGridCollisions = false;
            HorizontalFriction = 0;
            VerticalFriction = 0;
            HasGravity = false;
            Transform.Velocity = speed;
            Transform.Position = position;

            Timer.TriggerAfter(5000, Destroy);
        }
    }
}
