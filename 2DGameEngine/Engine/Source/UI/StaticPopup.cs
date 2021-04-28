using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonolithEngine.Engine.Source.Entities.Controller;
using MonolithEngine.Engine.Source.Graphics;
using MonolithEngine.Engine.Source.Scene;
using MonolithEngine.Engine.Source.Util;
using MonolithEngine.Entities;
using MonolithEngine.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine.Engine.Source.UI
{
    public class StaticPopup : Entity
    {
        
        private UserInputController input;


        public StaticPopup(AbstractScene scene, Texture2D texture, Vector2 position, float scale = 1, float timeout = 0, Keys continueButton = Keys.Space) : base(scene.LayerManager.UILayer, null, position)
        {
            input = new UserInputController();

            if (timeout == 0)
            {
                Scene.LayerManager.Paused = true;
                input.RegisterKeyPressAction(continueButton, (thumbstickPos) =>
                {
                    Scene.LayerManager.Paused = false;
                    Destroy();
                });
            }
            else
            {
                Timer.TriggerAfter(timeout, () => {
                    Destroy();
                });
            }

            Sprite s = new Sprite(this, texture, new Rectangle(0, 0, texture.Width, texture.Height));
            s.Scale = scale;
            AddComponent(s);

            Active = true;
            Visible = true;
        }



        public override void FixedUpdate()
        {
            input.Update();
            base.FixedUpdate();
        }

    }
}
