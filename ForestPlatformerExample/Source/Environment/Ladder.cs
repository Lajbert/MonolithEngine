using ForestPlatformerExample.Source.PlayerCharacter;
using MonolithEngine.Engine.Source.Entities.Abstract;
using MonolithEngine.Engine.Source.Physics.Collision;
using MonolithEngine.Engine.Source.Physics.Trigger;
using MonolithEngine.Entities;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using MonolithEngine.Engine.Source.Scene;

namespace ForestPlatformerExample.Source.Environment
{
    class Ladder : Entity
    {
        public Ladder(AbstractScene scene, Vector2 position, int width, int height) : base(scene.LayerManager.EntityLayer, null, position)
        {
            if (width == 0 || height == 0)
            {
                throw new Exception("Invalid ladder dimensions!");
            }

            AddTag("Environment");
            Active = true;

            AddComponent(new BoxTrigger(this, width, height, Vector2.Zero, "Ladder"));

        }

        public override void OnEnterTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                (otherEntity as Hero).EnterLadder(this);
            }
            base.OnEnterTrigger(triggerTag, otherEntity);
        }

        public override void OnLeaveTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                (otherEntity as Hero).LeaveLadder();
            }
            base.OnLeaveTrigger(triggerTag, otherEntity);
        }
    }
}
