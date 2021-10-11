using Microsoft.Xna.Framework;
using MonolithEngine;
using System;

namespace ForestPlatformerExample
{
    class IceTrigger: Entity
    {
        public IceTrigger(AbstractScene scene, int width, int height, Vector2 position) : base(scene.LayerManager.EntityLayer, null, position)
        {
            if (width == 0 || height == 0)
            {
                throw new Exception("Invalid ice trigger size");
            }
            Visible = false;
            Active = true;
            AddTag("Environment");
            AddComponent(new BoxTrigger(this, width, height, Vector2.Zero, tag: ""));
            AddTriggeredAgainst(typeof(Hero));
#if DEBUG
            Visible = true;
            (GetComponent<ITrigger>() as AbstractTrigger).DEBUG_DISPLAY_TRIGGER = true;
#endif
        }

        public override void OnEnterTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                (otherEntity as Hero).OnIce = true;
            }
            base.OnEnterTrigger(triggerTag, otherEntity);
        }

        public override void OnLeaveTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                (otherEntity as Hero).OnIce = false;
            }
            base.OnLeaveTrigger(triggerTag, otherEntity);
        }
    }
}
