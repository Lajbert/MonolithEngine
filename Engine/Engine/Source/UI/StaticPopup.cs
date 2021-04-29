using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonolithEngine
{
    public class StaticPopup : Entity
    {
        
        private UserInputController input;

        private SpriteFont font;

        private string text;

        private Color textColor;

        private float timeout;

        private Keys continueButton;

        public StaticPopup(AbstractScene scene, Vector2 position, float timeout = 0, Keys continueButton = Keys.Space) : base(scene.LayerManager.UILayer, null, position)
        {
            input = new UserInputController();

            this.timeout = timeout;

            this.continueButton = continueButton;

            Visible = false;

        }

        public void SetSprite(Texture2D texture, float scale)
        {
            Sprite s = new Sprite(this, texture, new Rectangle(0, 0, texture.Width, texture.Height));
            s.Scale = scale;
            AddComponent(s);
        }

        public void SetText(SpriteFont font, string text, Color textColor = default)
        {
            this.text = text;
            this.font = font;
            if (textColor == default)
            {
                this.textColor = Color.White;
            }
            else
            {
                this.textColor = textColor;
            }
        }

        public override void FixedUpdate()
        {
            input.Update();
            base.FixedUpdate();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (text != null)
            {
                spriteBatch.DrawString(font, text, Transform.Position, textColor);
            }
        }

        public void Display()
        {
            Active = true;
            Visible = true;

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
        }

    }
}
