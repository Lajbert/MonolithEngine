using ForestPlatformerExample.Source.PlayerCharacter;
using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Entities.Abstract;
using MonolithEngine.Engine.Source.Physics.Trigger;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Entities;
using MonolithEngine.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Environment
{
    class NextLevelTrigger : Entity
    {
        public NextLevelTrigger(AbstractScene scene, Vector2 position) : base(scene.LayerManager.EntityLayer, null, position)
        {
            Visible = false;
            Active = true;
            AddTag("Environment");
            int width = 256;
            int height = 256;
            AddComponent(new BoxTrigger(this, width, height, new Vector2(-width / 2, -height + Config.GRID), tag: ""));
#if DEBUG
            Visible = true;
            (GetComponent<ITrigger>() as AbstractTrigger).DEBUG_DISPLAY_TRIGGER = true;
#endif
        }

        public override void OnEnterTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                (otherEntity as Hero).LevelEndReached = true;
            }
            base.OnEnterTrigger(triggerTag, otherEntity);
        }

        public override void OnLeaveTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                if ((otherEntity as Hero).ReadyForNextLevel)
                {
                    Scene.Finish();
                } else
                {
                    (otherEntity as Hero).LevelEndReached = false;
                }
            }
            base.OnLeaveTrigger(triggerTag, otherEntity);
        }
    }
}
