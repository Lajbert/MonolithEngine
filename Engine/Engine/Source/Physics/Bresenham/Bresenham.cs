using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonolithEngine
{

    public class Bresenham
    {
		public static void GetLine(Vector2 a, Vector2 b, List<Vector2> result)
		{
			//x0: Int, y0: Int, x1: Int, y1: Int
			int x0 = (int)a.X;
			int y0 = (int)a.Y;
			int x1 = (int)b.X;
			int y1 = (int)b.Y;

			if (a == b)
			{
				return;
			}

			bool swapXY = FastAbs(y1 - y0) > FastAbs(x1 - x0);
			int tmp;
			if (swapXY)
			{
				// swap x and y
				tmp = x0; 
				x0 = y0; 
				y0 = tmp; // swap x0 and y0
				tmp = x1; 
				x1 = y1;
				y1 = tmp; // swap x1 and y1
			}

			if (x0 > x1)
			{
				// make sure x0 < x1
				tmp = x0; 
				x0 = x1; 
				x1 = tmp; // swap x0 and x1
				tmp = y0; 
				y0 = y1; 
				y1 = tmp; // swap y0 and y1
			}

			int deltaX = x1 - x0;
			int deltaY = (int)(FastAbs(y1 - y0));
			int error = (int)(deltaX / 2);
			int y = y0;
			int yStep = (y0 < y1) ? 1 : -1;
			if (swapXY)
			{
				// Y / X
				for (int x = x0; x <= x1; x++)
				{
					result.Add(new Vector2(y, x));
					error -= deltaY;
					if (error < 0)
					{
						y = y + yStep;
						error = error + deltaX;
					}
				}
			}
			else 
			{
				// X / Y
				for (int x = x0; x <= x1; x++)
				{
					result.Add(new Vector2(x, y));
					error -= deltaY;
					if (error < 0)
					{
						y = y + yStep;
						error = error + deltaX;
					}
				}
			}
        }

		public static bool CanLinePass(Vector2 a, Vector2 b, Func<int, int, bool> isRayBlocked)
		{
			//x0: Int, y0: Int, x1: Int, y1: Int
			int x0 = (int)a.X;
			int y0 = (int)a.Y;
			int x1 = (int)b.X;
			int y1 = (int)b.Y;

			if (a == b)
			{
				return false;
			}

			bool swapXY = FastAbs(y1 - y0) > FastAbs(x1 - x0);
			int tmp;
			if (swapXY)
			{
				// swap x and y
				tmp = x0;
				x0 = y0;
				y0 = tmp; // swap x0 and y0
				tmp = x1;
				x1 = y1;
				y1 = tmp; // swap x1 and y1
			}

			if (x0 > x1)
			{
				// make sure x0 < x1
				tmp = x0;
				x0 = x1;
				x1 = tmp; // swap x0 and x1
				tmp = y0;
				y0 = y1;
				y1 = tmp; // swap y0 and y1
			}

			int deltaX = x1 - x0;
			int deltaY = (int)(FastAbs(y1 - y0));
			int error = (int)(deltaX / 2);
			int y = y0;
			int yStep = (y0 < y1) ? 1 : -1;
			if (swapXY)
			{
				// Y / X
				for (int x = x0; x <= x1; x++)
				{
					if (isRayBlocked(y, x))
                    {
						return false;
					}

					
					error -= deltaY;
					if (error < 0)
					{
						y = y + yStep;
						error = error + deltaX;
					}
				}
			}
			else
			{
				// X / Y
				for (int x = x0; x <= x1; x++)
				{
					if (isRayBlocked(x, y))
                    {
						return false;
					}
						
					error -= deltaY;
					if (error < 0)
					{
						y = y + yStep;
						error = error + deltaX;
					}
				}
			}
			return true;
		}

		static int FastAbs(int v)  {
	        return (v ^ (v >> 31)) - (v >> 31);
        }
    }

#if DEBUG
	public class BresenhamLine : Entity
	{
		List<Vector2> line = new List<Vector2>();
		Entity other;
		public BresenhamLine(AbstractScene scene, Entity owner, Entity other) : base(scene.LayerManager.EntityLayer, owner, Vector2.Zero)
		{
			this.other = other;
			Active = true;
		}

		bool canRayPass;
		public override void Update()
		{
			base.Update();
			line.Clear();
			Bresenham.GetLine(Parent.Transform.Position + new Vector2(0, -16), other.Transform.Position + new Vector2(0, -16), line);
			canRayPass = Bresenham.CanLinePass(Parent.Transform.Position + new Vector2(0, -16), other.Transform.Position + new Vector2(0, -16), (x, y) => {
				return Scene.GridCollisionChecker.HasBlockingColliderAt(new Vector2(x / Config.GRID, y / Config.GRID), Direction.CENTER);
			});
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			if (canRayPass)
			{
				foreach (Vector2 point in line)
				{
					spriteBatch.Draw(AssetUtil.CreateRectangle(1, Color.Black), point, Color.White);
				}
			}
			else
			{
				foreach (Vector2 point in line)
				{
					spriteBatch.Draw(AssetUtil.CreateRectangle(1, Color.Red), point, Color.White);
				}
			}

		}
	}
#endif
}
