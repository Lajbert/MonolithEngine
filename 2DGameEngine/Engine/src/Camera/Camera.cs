using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.src.Camera
{
	class Camera
	{
		public Entity target;

		private Vector2 position;
		//public float x;
		//public float y;

		public Vector2 direction;
		//public float dx;
		//public float dy;
		//public int wid;
		//public int hei;

		float bumpOffX = 0f;
		float bumpOffY = 0f;

		public float targetTrackOffX = 0f;
		public float targetTrackOffY = 0f;
		public float zoom = 1f;

		float gameMeW = Constants.RES_W;
		float gameMeH = Constants.RES_H;
		float levelGridCountW = (float)Math.Floor((decimal)Constants.RES_W);
		float levelGridCountH = (float)Math.Floor((decimal)Constants.RES_H);

		private float shakePower = 1.5f;

		private float SCALE = 1;
		private bool SCROLL = true;

		private bool shake = false;

		private float elapsedTime;

		RootContainer root;

		public Camera() {
			//x = y = 0;
			//dx = dy = 0;
			position = Vector2.Zero;
			direction = Vector2.Zero;
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
			{
				recenter();
			}
		}

		public void stopTracking()
		{
			target = null;
		}

		public void recenter()
		{
			if (target != null) {
				position = new Vector2(target.GetPosition().X + targetTrackOffX, target.GetPosition().Y + targetTrackOffY);
				//position.X = target.GetPosition().X + targetTrackOffX;
				//position.Y = target.GetPosition().Y + targetTrackOffY;
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
			elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds / Constants.CAMERA_TIME_MULTIPLIER;
			// Follow target entity
			if (target != null)
			{
				float tx = target.GetPosition().X + targetTrackOffX;
				float ty = target.GetPosition().Y + targetTrackOffY;

				float d = dist(position.X, position.Y, tx, ty);
				if (d >= Constants.CAMERA_DEADZONE)
				{
					float a = (float)Math.Atan2(ty - position.Y, tx - position.X);
					direction.X += (float)Math.Cos(a) * (d - Constants.CAMERA_DEADZONE) * Constants.CAMERA_FOLLOW_DELAY * elapsedTime;
					direction.Y += (float)Math.Sin(a) * (d - Constants.CAMERA_DEADZONE) * Constants.CAMERA_FOLLOW_DELAY * elapsedTime;
				}
			}

			float frict = 0.89f;
			position.X += direction.X * elapsedTime;
			direction.X *= (float)Math.Pow(frict, elapsedTime);

			position.Y += direction.Y * elapsedTime;
			direction.Y *= (float)Math.Pow(frict, elapsedTime);
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
				root = RootContainer.Instance;

				// Update scroller
				if (get_wid() / zoom < levelGridCountW * Constants.GRID)
					root.X = (float)(-position.X * zoom + get_wid() * 0.5);
				else
					root.X = (float)(get_wid() * 0.5 / zoom - levelGridCountW * 0.5 * Constants.GRID);

				if (get_hei() / zoom < levelGridCountH * Constants.GRID)
					root.Y = (float)(-position.Y * zoom + get_hei() * 0.5);
				else
					root.Y = (float)(get_hei() * 0.5 / zoom - levelGridCountH * 0.5 * Constants.GRID);

				// Clamp
				float pad = Constants.GRID * 2;
				if (get_wid() < levelGridCountW * Constants.GRID * zoom)
					root.X = fclamp(root.X, get_wid() - levelGridCountW * Constants.GRID * zoom + pad, -pad);
				if (get_hei() < levelGridCountH * Constants.GRID * zoom)
					root.Y = fclamp(root.Y, get_hei() - levelGridCountH * Constants.GRID * zoom + pad, -pad);

				// Bumps friction
				bumpOffX *= (float)Math.Pow(0.75, tmod);
				bumpOffY *= (float)Math.Pow(0.75, tmod);

				// Bump
				root.X += bumpOffX;
				root.Y += bumpOffY;

				// Shakes
				if (shake)
				{
					root.X += (float)(Math.Cos(gameTime.TotalGameTime.TotalMilliseconds * 1.1) * 2.5 * shakePower * 0.5f);
					root.Y += (float)(Math.Sin(0.3 + gameTime.TotalGameTime.TotalMilliseconds * 1.7) * 2.5 * shakePower * 0.5f);
				}

				// Scaling
				root.X *= SCALE;
				root.Y *= SCALE;

				// Rounding
				root.X = (float)Math.Round(root.X);
				root.Y = (float)Math.Round(root.Y);

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
