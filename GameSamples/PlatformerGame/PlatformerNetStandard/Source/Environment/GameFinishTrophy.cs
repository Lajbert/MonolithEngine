using Microsoft.Xna.Framework;
using MonolithEngine;

namespace ForestPlatformerExample
{
    class GameFinishTrophy : PhysicalEntity
    {
        public GameFinishTrophy(AbstractScene scene, Vector2 position, Vector2 pivot) : base(scene.LayerManager.EntityLayer, null, position)
        {
            AddTag("FinishTropy");

            HasGravity = false;

            DrawPriority = 5;

            Pivot = pivot;

            CollisionOffsetBottom = 1f;

            AddComponent(new Sprite(this, Assets.GetTexture("FinishedTrophy")));
            Rectangle SourceRectangle = GetComponent<Sprite>().SourceRectangle;
            Vector2 offset = new Vector2(SourceRectangle.Width * -Pivot.X, SourceRectangle.Height * -Pivot.Y);
            AddComponent(new BoxCollisionComponent(this, SourceRectangle.Width, SourceRectangle.Height, offset));
            Active = true;
            Visible = true;
#if DEBUG
            DEBUG_SHOW_PIVOT = true;
            DEBUG_SHOW_COLLIDER = true;
            (GetCollisionComponent() as BoxCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
#endif
        }
    }
}
