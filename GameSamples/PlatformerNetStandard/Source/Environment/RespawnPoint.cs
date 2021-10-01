﻿using Microsoft.Xna.Framework;
using MonolithEngine;
using System;

namespace ForestPlatformerExample
{
    class RespawnPoint : Entity
    {
        public RespawnPoint(AbstractScene scene, int width, int height, Vector2 position) : base (scene.LayerManager.EntityLayer, null, position)
        {
            if (width == 0 || height == 0)
            {
                throw new Exception("Invalid respawn point trigger size");
            }
            Visible = false;
            Active = true;
            AddTag("Environment");
            AddComponent(new BoxTrigger(this, width, height, new Vector2(-width / 2, -height + Config.GRID), tag: ""));
#if DEBUG
            //Visible = true;
            //(GetComponent<ITrigger>() as AbstractTrigger).DEBUG_DISPLAY_TRIGGER = true;
#endif
        }

        public override void OnEnterTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                (otherEntity as Hero).LastSpawnPoint = Transform.Position;
            }
            base.OnEnterTrigger(triggerTag, otherEntity);
        }
    }
}
