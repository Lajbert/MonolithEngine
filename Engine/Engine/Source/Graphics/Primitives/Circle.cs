using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonolithEngine
{
    /// <summary>
    /// Represents a circle. 
    /// WARNING: ugly and bad performance, only for debugging purposes!
    /// </summary>
    public class Circle : Entity
    {
        private Vector2 center;
        private Color color;
        private float radius;
        private Vector2 offset;

        public Circle(AbstractScene scene, Entity parent, Vector2 center, int radius, Color color) : base(scene.LayerManager.EntityLayer, parent, center)
        {
            SetSprite(AssetUtil.CreateCircle(radius, color));
            this.color = color;
            this.center = center;
            this.radius = radius;
            this.offset = new Vector2(radius, radius) / 2;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            GetComponent<Sprite>().DrawOffset -= offset;
            base.Draw(spriteBatch);
        }
    }
}
