using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonolithEngine
{
    /// <summary>
    /// Popus are special game entities. They represent any text or image
    /// that you want to see in your game, like a text fot tutorials, 
    /// subtitles for conversations, etc...
    /// Static popup class doens't follow any entities, they stick to one position,
    /// move with the camera and can pause the game while on screen waiting for a key
    /// input to continue or automatically disappear after a delay.
    /// </summary>
    public class StaticPopup : Entity
    {
        
        private SpriteFont font;

        private string text;

        private Color textColor;

        private float timeout;

        private Keys continueButton;

        public StaticPopup(AbstractScene scene, Vector2 position, float timeout = 0, Keys continueButton = Keys.Space) : base(scene.LayerManager.UILayer, null, position)
        {
            AddComponent(new UserInputController());

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
                GetComponent<UserInputController>().RegisterKeyPressAction(continueButton, (thumbstickPos) =>
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
