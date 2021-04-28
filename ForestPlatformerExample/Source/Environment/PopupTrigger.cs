﻿using ForestPlatformerExample.Source.PlayerCharacter;
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
                    DisplayControlsText();
                }
                else if (textName == "BoxThrow")
                {
                    DisplayBoxThrowText();
                }
                else if (textName == "SpikeReminder")
                {
                    DisplaySpikesReminder();
                }
            }
            base.OnEnterTrigger(triggerTag, otherEntity);
        }

        private void DisplayControlsText()
        {
            StaticPopup popup = new StaticPopup(Scene, Transform.Position + new Vector2(0, -120), continueButton: Keys.Enter);
            string text = " Controls:\n LEFT/RIGHT Arrows: Walk\n UP Arrow: Jump\n DOWN Arrow: Descend from platform\n SPACE: Punch/Throw box\n Left/Right SHIFT: Pick up/Put down box\n Left/Right CONTROL: Slide" +
                "\n\n Kill enemies by jumping on their heads,\n punching them or throwing boxes at them." +
                "\n\n You won't take any damage while sliding." +
                "\n\n [PRESS ENTER TO CONTINUE]";
            popup.SetText(Assets.GetFont("InGameText"), text, Color.White);
            popup.SetSprite(Assets.CreateRectangle(230, 210, Color.Black));
            Destroy();
        }

        private void DisplayBoxThrowText()
        {
            StaticPopup popup = new StaticPopup(Scene, Transform.Position + new Vector2(0, -50), continueButton: Keys.Enter);
            string text = " Reminder: you can pick up boxes with SHIFT,\n then press SPACE to throw them at enemies.\n You can jump while holding a box!" +
            "\n\n [PRESS ENTER TO CONTINUE]";
            popup.SetText(Assets.GetFont("InGameText"), text, Color.White);
            popup.SetSprite(Assets.CreateRectangle(270, 80, Color.Black));
            Destroy();
        }

        private void DisplaySpikesReminder()
        {
            StaticPopup popup = new StaticPopup(Scene, Transform.Position + new Vector2(0, -10), 6000, Keys.Enter);
            string text = " Reminder: Press CONTROL to slide through spikes\n without taking damage.";
            popup.SetText(Assets.GetFont("InGameText"), text, Color.White);
            popup.SetSprite(Assets.CreateRectangle(270, 30, Color.Black));
            Destroy();
        }
    }
}