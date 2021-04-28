using ForestPlatformerExample.Source.PlayerCharacter;
using Microsoft.Xna.Framework;
using MonolithEngine;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Entities.Abstract;
using MonolithEngine.Engine.Source.Graphics;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Physics.Trigger;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Entities;
using MonolithEngine.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Environment
{
    class GameFinishTrophy : PhysicalEntity
    {
        public GameFinishTrophy(AbstractScene scene, Vector2 position) : base(scene.LayerManager.EntityLayer, null, position)
        {
            AddTag("FinishTropy");

            DrawPriority = 5;

            Pivot = new Vector2(0.5f, 1f);

            CollisionOffsetBottom = 1f;

            AddComponent(new Sprite(this, Assets.GetTexture("FinishedTrophy")));
            Rectangle SourceRectangle = GetComponent<Sprite>().SourceRectangle;
            Vector2 offset = new Vector2(SourceRectangle.Width * Pivot.X, SourceRectangle.Height * Pivot.Y);
            AddComponent(new BoxCollisionComponent(this, SourceRectangle.Width, SourceRectangle.Height, -offset));
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
