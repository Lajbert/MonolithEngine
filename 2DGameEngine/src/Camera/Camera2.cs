using _2DGameEngine.Entities;
using _2DGameEngine.Global;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace _2DGameEngine.src.Camera
{
	class Camera2
	{
		public Entity target;
		public float x;
		public float y;
		public float dx;
		public float dy;
		//public int wid;
		//public int hei;

		float bumpOffX = 0f;
		float bumpOffY = 0f;

		public float targetTrackOffX = 0f;
		public float targetTrackOffY = 0f;
		public float zoom = 1f;

		float gameMeW = 5000;
		float gameMeH = 5000;
		float levelcWid = 5000;
		float levelcHei = 5000;

		private float shakePower = 1.0f;

		private float SCALE = 1;
		private bool SCROLL = true;

		RootContainer scroller;

		public Camera2() {
			x = y = 0;
			dx = dy = 0;
		}

		private float get_wid()
		{
			return (float)Math.Ceiling(gameMeW / SCALE);
		}

		private float get_hei()
		{
			return (float)Math.Ceiling(gameMeH / SCALE);
		}

		public void trackTarget(Entity e, bool immediate, float xOff = 0f, float yOff = 0f)
		{
			targetTrackOffX = xOff;
			targetTrackOffY = yOff;
			target = e;
			if (immediate)
				recenter();
		}

		public void stopTracking()
		{
			target = null;
		}

		public void recenter()
		{
			if (target != null) {

				x = target.GetGridCoord().X + targetTrackOffX;
				y = target.GetGridCoord().Y + targetTrackOffY;
			}
		}

		public float scrollerToGlobalX(float v) {
			return v * SCALE + RootContainer.Instance.GetRootPosition().X;
		}
		public float scrollerToGlobalY(float v) {
			return v * SCALE + RootContainer.Instance.GetRootPosition().Y;
		}

		/*public void shakeS(float t, float pow = 1.0)
		{
			cd.setS("shaking", t, false);
			shakePower = pow;
		}*/

		public void update(GameTime gameTime)
		{

			float tmod = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			// Follow target entity
			if (target != null)
			{
				float s = 0.006f;
				float deadZone = 1;
				float tx = target.GetGridCoord().X + targetTrackOffX;
				float ty = target.GetGridCoord().Y + targetTrackOffY;

				float d = dist(x, y, tx, ty);
				if (d >= deadZone)
				{
					float a = (float)Math.Atan2(ty - y, tx - x);
					dx += (float)Math.Cos(a) * (d - deadZone) * s * tmod;
					dy += (float)Math.Sin(a) * (d - deadZone) * s * tmod;
				}
			}

			float frict = 0.89f;
			x += dx * tmod;
			dx *= (float)Math.Pow(frict, tmod);

			y += dy * tmod;
			dy *= (float)Math.Pow(frict, tmod);
		}

		public void bumpAng(float a, float dist)
		{
			bumpOffX += (float)Math.Cos(a) * dist;
			bumpOffY += (float)Math.Sin(a) * dist;
		}

		public void bump(float x, float y)
		{
			bumpOffX += x;
			bumpOffY += y;
		}


		public void postUpdate(GameTime gameTime)
		{
			float tmod = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			if (SCROLL)
			{
				scroller = RootContainer.Instance;

				// Update scroller
				if (get_wid() / zoom < levelcWid * Constants.GRID)
					scroller.X = (float)(-x * zoom + get_wid() * 0.5);
				else
					scroller.X = (float)(get_wid() * 0.5 / zoom - levelcWid * 0.5 * Constants.GRID);

				if (get_hei() / zoom < levelcHei * Constants.GRID)
					scroller.Y = (float)(-y * zoom + get_hei() * 0.5);
				else
					scroller.Y = (float)(get_hei() * 0.5 / zoom - levelcHei * 0.5 * Constants.GRID);

				// Clamp
				float pad = Constants.GRID * 2;
				if (get_wid() < levelcWid * Constants.GRID * zoom)
					scroller.X = fclamp(scroller.X, get_wid() - levelcWid * Constants.GRID * zoom + pad, -pad);
				if (get_hei() < levelcHei * Constants.GRID * zoom)
					scroller.Y = fclamp(scroller.Y, get_hei() - levelcHei * Constants.GRID * zoom + pad, -pad);

				// Bumps friction
				bumpOffX *= (float)Math.Pow(0.75, tmod);
				bumpOffY *= (float)Math.Pow(0.75, tmod);

				// Bump
				scroller.X += bumpOffX;
				scroller.Y += bumpOffY;

				// Shakes
				/*if (cd.has("shaking"))
				{
					scroller.X += Math.cos(ftime * 1.1) * 2.5 * shakePower * cd.getRatio("shaking");
					scroller.Y += Math.sin(0.3 + ftime * 1.7) * 2.5 * shakePower * cd.getRatio("shaking");
				}*/

				// Scaling
				scroller.X *= SCALE;
				scroller.Y *= SCALE;

				// Rounding
				scroller.X = (float)Math.Round(scroller.X);
				scroller.Y = (float)Math.Round(scroller.Y);

				// Zoom
				//scroller.setScale(SCALE * zoom);
			}
		}

		public float dist(float ax, float ay, float bx, float by)
		{
			return (float)Math.Sqrt(distSqr(ax, ay, bx, by));
		}

		public static float distSqr(float ax, float ay, float bx, float by) 
		{
			return (ax-bx)*(ax-bx) + (ay-by)*(ay-by);
		}

		public float fclamp(float x, float min, float max) 
		{
			return (x<min) ? min : (x > max) ? max : x;
		}

	}
}
