using Microsoft.Xna.Framework;
using MonolithEngine;
using System;

namespace ForestPlatformerExample
{
    class Fan : Entity 
    {

        public int ForceFieldHeight;

        private Vector2 drawOffset = new Vector2(7, 8);

        public Fan(AbstractScene scene, Vector2 position, int forceFeildHeight = 256) : base (scene.LayerManager.EntityLayer, null, position)
        {
            if (forceFeildHeight <= 0)
            {
                throw new Exception("Incorrect force field height for fan!");
            }
            AddTag("Fan");
            ForceFieldHeight = forceFeildHeight;
            AnimationStateMachine animations = new AnimationStateMachine();
            animations.Offset = drawOffset;
            AddComponent(animations);
            SpriteSheetAnimation fanAnim = new SpriteSheetAnimation(this, Assets.GetTexture("FanAnim"), 24, 8, 24);
            fanAnim.Scale = 2f;
            animations.RegisterAnimation("FanWorking", fanAnim);

            Vector2 offset = new Vector2(-32, -ForceFieldHeight);

            AddComponent(new BoxTrigger(this, 64, ForceFieldHeight, positionOffset: offset + drawOffset, tag: ""));

            AddComponent(new BoxCollisionComponent(this, 46, 8, new Vector2(-14, 0)));

#if DEBUG
            //(GetComponent<ITrigger>() as BoxTrigger).DEBUG_DISPLAY_TRIGGER = true;
            //(GetCollisionComponent() as BoxCollisionComponent).DEBUG_DISPLAY_COLLISION = true;
#endif

            Active = true;
        }

        public override void OnEnterTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                (otherEntity as Hero).EnterFanArea(this);
            }
            base.OnEnterTrigger(triggerTag, otherEntity);
        }

        public override void OnLeaveTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                (otherEntity as Hero).LeaveFanArea();
            }
            base.OnEnterTrigger(triggerTag, otherEntity);
        }
    }
}
