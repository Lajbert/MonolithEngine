using Microsoft.Xna.Framework;
using MonolithEngine;

namespace ForestPlatformerExample
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
            if (otherEntity is Saw)
            {
                (otherEntity as Saw).ChangeDirection();
            }
            base.OnLeaveTrigger(triggerTag, otherEntity);
        }
    }
}
