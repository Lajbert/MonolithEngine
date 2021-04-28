using ForestPlatformerExample.Source.PlayerCharacter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonolithEngine.Engine.Source.Asset;
using MonolithEngine.Engine.Source.Entities.Abstract;
using MonolithEngine.Engine.Source.Physics.Trigger;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.UI;
using MonolithEngine.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForestPlatformerExample.Source.Environment
{
    class PopupTrigger : Entity
    {

        private string textName;

        public PopupTrigger(AbstractScene scene, Vector2 position, int width, int height, string textName) : base(scene.LayerManager.EntityLayer, null, position)
        {
            Visible = false;
            Active = true;
            AddTag("Environment");
            AddComponent(new BoxTrigger(this, width, height, Vector2.Zero, tag: ""));
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
                if (textName == "Controls")
                {
                    DisplayControlsTrigger();
                }
                else if (textName == "BoxThrow")
                {
                    DisplayBoxThrowTrigger();
                }
            }
            base.OnEnterTrigger(triggerTag, otherEntity);
        }

        private void DisplayControlsTrigger()
        {
            StaticPopup popup = new StaticPopup(Scene, Transform.Position + new Vector2(0, -120), continueButton: Keys.Enter);
            string text = " Controls:\n LEFT/RIGHT Arrows: Walk\n UP Arrow: Jump\n DOWN Arrow: Descend from platform\n SPACE: Punch/Throw box\n Left/Right SHIFT: Pick up/Put down box" +
                "\n\n Kill enemies by jumping on their heads,\n punching them or throwing boxes at them." +
                "\n\n [PRESS ENTER TO CONTINUE]";
            popup.SetText(Assets.GetFont("InGameText"), text, Color.White);
            popup.SetSprite(Assets.CreateRectangle(230, 180, Color.Black));
            Destroy();
        }

        private void DisplayBoxThrowTrigger()
        {
            StaticPopup popup = new StaticPopup(Scene, Transform.Position + new Vector2(0, -50), continueButton: Keys.Enter);
            string text = " Reminder: you can pick up boxes with SPACE,\n then press SPACE again to throw them at enemies.\n You can jump while holding a box!" +
            "\n\n [PRESS ENTER TO CONTINUE]";
            popup.SetText(Assets.GetFont("InGameText"), text, Color.White);
            popup.SetSprite(Assets.CreateRectangle(270, 80, Color.Black));
            Destroy();
        }
    }
}