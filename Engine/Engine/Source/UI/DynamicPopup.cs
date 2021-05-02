using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonolithEngine
{
    /// <summary>
    /// Popus are special game entities. They represent any text or image
    /// that you want to see in your game, like a text fot tutorials, 
    /// subtitles for conversations, etc...
    /// Doesn't move with the camera view and affected by the zoom level,
    /// can follow a target entity and can disappear after a delay.
    /// </summary>
    class DynamicPopup : Entity
    {

        private UserInputController input;

        public DynamicPopup(AbstractScene scene, Texture2D texture, Vector2 position, Entity follow, float scale = 1, float timeout = 0) : base(scene.LayerManager.EntityLayer, follow, position)
        {
            input = new UserInputController();

            Timer.TriggerAfter(timeout, () => {
                Destroy();
            });

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
