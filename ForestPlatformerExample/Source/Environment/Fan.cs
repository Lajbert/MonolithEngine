using ForestPlatformerExample.Source.PlayerCharacter;
using Microsoft.Xna.Framework;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Entities.Abstract;
using MonolithEngine.Engine.Source.Entities.Animations;
using MonolithEngine.Engine.Source.Physics.Trigger;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Entities;
using MonolithEngine.Source.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Environment
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
            ForceFieldHeight = forceFeildHeight;
            AnimationStateMachine animations = new AnimationStateMachine();
            animations.Offset = drawOffset;
            AddComponent(animations);
            SpriteSheetAnimation fanAnim = new SpriteSheetAnimation(this, Assets.GetTexture("FanAnim"), 1, 4, 4, 24, 8, 24);
            fanAnim.Scale = 2f;
            animations.RegisterAnimation("FanWorking", fanAnim);

            Vector2 offset = new Vector2(-32, -ForceFieldHeight);

            AddComponent(new BoxTrigger(this, 64, ForceFieldHeight, positionOffset: offset + drawOffset, tag: ""));

#if DEBUG
            //(GetComponent<ITrigger>() as BoxTrigger).DEBUG_DISPLAY_TRIGGER = true;
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
