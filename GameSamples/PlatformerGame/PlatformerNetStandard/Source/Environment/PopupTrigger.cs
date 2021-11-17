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

            if (textName == "Controls")
            {
                popup = new StaticPopup(Scene, Transform.Position + new Vector2(0, -120), 6000, continueButton: Keys.Enter);
                string text = " Controls:\n LEFT/RIGHT Arrows: Walk\n UP Arrow: Jump\n DOWN Arrow: Descend from platform\n SPACE: Punch/Throw box" +
                    "\n Left/Right SHIFT: Pick up/Put down box\n Left/Right CONTROL: Slide\n Mouse Wheel Up/Down: Zoom In/Out" +
                    "\n\n Kill enemies by jumping on their heads,\n punching them or throwing boxes at them." +
                    "\n\n You won't take any damage while sliding." +
                    "\n\n [PRESS ENTER TO CONTINUE]";
                popup.SetText(Assets.GetFont("InGameText"), text, Color.White);
                popup.SetSprite(new MonolithTexture(Assets.CreateRectangle(230, 230, Color.Black)));
            }
            else if (textName == "BoxThrow")
            {
                popup = new StaticPopup(Scene, Transform.Position + new Vector2(0, -50), 6000, continueButton: Keys.Enter);
                string text = " Reminder: you can pick up boxes with SHIFT,\n then press SPACE to throw them at enemies.\n You can jump while holding a box!" +
                "\n\n [PRESS ENTER TO CONTINUE]";
                popup.SetText(Assets.GetFont("InGameText"), text, Color.White);
                popup.SetSprite(new MonolithTexture(Assets.CreateRectangle(270, 80, Color.Black)));
            }
            else if (textName == "SpikeReminder")
            {
                popup = new StaticPopup(Scene, Transform.Position + new Vector2(0, -10), 6000, Keys.Enter);
                string text = " Reminder: Press CONTROL to slide through spikes\n without taking damage.";
                popup.SetText(Assets.GetFont("InGameText"), text, Color.White);
                popup.SetSprite(new MonolithTexture(Assets.CreateRectangle(270, 30, Color.Black)));
            }

            this.textName = textName;
        }

        public override void OnEnterTrigger(string triggerTag, IGameObject otherEntity)
        {
            if (otherEntity is Hero)
            {
                AudioEngine.StopSoundEffects();
                popup.Display();
                Destroy();
            }
            base.OnEnterTrigger(triggerTag, otherEntity);
        }
    }
}