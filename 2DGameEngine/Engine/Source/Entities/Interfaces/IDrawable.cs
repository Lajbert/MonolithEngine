using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine2D.Entities.Interfaces
{
    public interface IDrawable
    {

        public void PreDraw(SpriteBatch spriteBatch, GameTime gameTime);
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime);
        public void PostDraw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
