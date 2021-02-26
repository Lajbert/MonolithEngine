using ForestPlatformerExample.Source.PlayerCharacter;
using GameEngine2D.Engine.Source.Entities.Abstract;
using GameEngine2D.Engine.Source.Physics.Collision;
using GameEngine2D.Engine.Source.Physics.Trigger;
using GameEngine2D.Entities;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Environment
{
    class Ladder : Entity
    {
        public Ladder(Vector2 position, int width, int height) : base(LayerManager.Instance.EntityLayer, null, position)
        {
            if (width == 0 || height == 0)
            {
                throw new Exception("Invalid ladder dimensions!");
            }

            AddTag("Ladder");
            AddComponent(new BoxTrigger(width, height, Vector2.Zero, "Ladder", true));

        }

        public override void OnEnterTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                (otherEntity as Hero).OnLadder = true;
            }
            base.OnEnterTrigger(triggerTag, otherEntity);
        }

        public override void OnLeaveTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                (otherEntity as Hero).OnLadder = false;
                (otherEntity as Hero).LeaveLadder();
                //TODO
                Logger.Warn("REDESIGN: Move the LeaveLadder() somehow fully to the Hero class, this trigger should only switch booleans!");
            }
            base.OnLeaveTrigger(triggerTag, otherEntity);
        }
    }
}
