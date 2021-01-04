using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine2D.Entities.Interfaces
{
    public interface IDrawable
    {

        public void PreDraw(GameTime gameTime);
        public void Draw(GameTime gameTime);
        public void PostDraw(GameTime gameTime);
    }
}
