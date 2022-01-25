using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonolithEngine;

namespace ForestPlatformerExample
{
    class PopupTrigger : Entity
    {

        private string textName;

        StaticPopup popup;

        public PopupTrigger(AbstractScene scene, Vector2 position, int width, int height, string textName) : base(scene.LayerManager.EntityLayer, null, position)
        {
            Visible = false;
            Active = true;
            AddTag("Environment");
            AddComponent(new BoxTrigger(this, width, height, Vector2.Zero, tag: ""));
            AddTriggeredAgainst(typeof(Hero));
#if DEBUG
            Visible = true;
            (GetComponent<ITrigger>() as AbstractTrigger).DEBUG_DISPLAY_TRIGGER = true;
#endif
            this.textName = textName;
        }

        public override void OnEnterTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                AudioEngine.StopSoundEffects();
                if (textName == "Controls")
                {
                    new ControlsPopup(Scene, new Vector2(50, 50));
                }
                else if (textName == "Box")
                {
                    new BoxTutorialPopup(Scene, new Vector2(50, 50));
                }
                else if (textName == "SpikeReminder")
                {
                    new SlideTutorialPopup(Scene, new Vector2(50, 50));
                }
                Destroy();
            }
            base.OnEnterTrigger(triggerTag, otherEntity);
        }
    }
}