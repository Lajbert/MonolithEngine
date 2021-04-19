using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Entities.Abstract;
using MonolithEngine.Engine.Source.Physics.Trigger;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Entities.Traps
{
    class SawPath : Entity
    {
        public SawPath(AbstractScene scene, Vector2 position, int width, int height) : base(scene.LayerManager.EntityLayer, null, position)
        {
            Visible = false;
            Active = true;
            AddComponent(new BoxTrigger(this, width, height, Vector2.Zero, tag: ""));
#if DEBUG
            Visible = true;
            (GetComponent<ITrigger>() as AbstractTrigger).DEBUG_DISPLAY_TRIGGER = true;
#endif
        }

        public override void OnEnterTrigger(string triggerTag, IGameObject otherEntity)
        {
            base.OnEnterTrigger(triggerTag, otherEntity);
        }

        public override void OnLeaveTrigger(string triggerTag, IGameObject otherEntity)
        {
            base.OnLeaveTrigger(triggerTag, otherEntity);
        }
    }
}
