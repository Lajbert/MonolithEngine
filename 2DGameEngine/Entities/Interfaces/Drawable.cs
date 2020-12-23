using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace _2DGameEngine.Entities.Interfaces
{
    interface Drawable
    {
        Texture2D sprite { get; set; }
        public void Draw(GameTime gameTime);
    }
}
