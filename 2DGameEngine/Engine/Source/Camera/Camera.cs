using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Source.Camera
{
	public class Camera
	{
		public Entity target { get; set; }

		private Vector2 position;
		//public float x;
		//public float y;

		public Vector2 direction;
		//public float dx;
		//public float dy;
		//public int wid;
		//public int hei;

		private float bumpOffX = 0f;
		private float bumpOffY = 0f;

		private float targetTrackOffX = 0f;
		private float targetTrackOffY = 0f;

		public float TargetTrackOffX { 

			get => targetTrackOffX;
			
			set => targetTrackOffX = value;
		}

		public float TargetTrackOffY
		{

			get => targetTrackOffY;

			set => targetTrackOffY = value;
		}

		public float zoom = 1f;

		private float gameMeW = Config.RES_W;
		private float gameMeH = Config.RES_H;
		public float LevelGridCountW = (float)Math.Floor((decimal)Config.RES_W);
		public float LevelGridCountH = (float)Math.Floor((decimal)Config.RES_H);

		private float shakePower = 1.5f;
		private float shakeStarted = 0f;
		private float shakeDuration = 0f;

		private float SCALE = 1;
		private bool SCROLL = true;

		private bool shake = false;

		private float elapsedTime;

		private float tx;
		private float ty;
		private float d;

		float frict = 0.89f;

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
			TargetTrackOffX = xOff;
			TargetTrackOffY = yOff;
			target = e;
			if (immediate)
			{
				Recenter();
			}
		}

		public void stopTracking()
		{
			target = null;
		}

		public void Recenter()
		{
			if (target != null) {
				position = new Vector2(target.Position.X + TargetTrackOffX, target.Position.Y + TargetTrackOffY);
				//position.X = target.GetPosition().X + targetTrackOffX;
				//position.Y = target.GetPosition().Y + targetTrackOffY;
			}
		}

		public float scrollerToGlobalX(float v) {
			return v * SCALE + RootContainer.Instance.Position.X;
		}
		public float scrollerToGlobalY(float v) {
			return v * SCALE + RootContainer.Instance.Position.Y;
		}

		public void Shake(float power = 1, float duration = 1)
        {
			shakePower = power;
			shakeDuration = duration;
			shake = true;
        }

		/*public void shakeS(float t, float pow = 1.0)
		{
			cd.setS("shaking", t, false);
			shakePower = pow;
		}*/

		public void update(GameTime gameTime)
		{
			elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds / Config.CAMERA_TIME_MULTIPLIER;
			// Follow target entity
			if (target != null)
			{
				tx = target.Position.X + TargetTrackOffX;
				ty = target.Position.Y + TargetTrackOffY;

				d = dist(position.X, position.Y, tx, ty);
				if (d >= Config.CAMERA_DEADZONE)
				{
					float a = (float)Math.Atan2(ty - position.Y, tx - position.X);
					direction.X += (float)Math.Cos(a) * (d - Config.CAMERA_DEADZONE) * Config.CAMERA_FOLLOW_DELAY * elapsedTime;
					direction.Y += (float)Math.Sin(a) * (d - Config.CAMERA_DEADZONE) * Config.CAMERA_FOLLOW_DELAY * elapsedTime;
				}
			}

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
				if (get_wid() / zoom < LevelGridCountW * Config.GRID)
					root.X = (float)(-position.X * zoom + get_wid() * 0.5);
				else
					root.X = (float)(get_wid() * 0.5 / zoom - LevelGridCountW * 0.5 * Config.GRID);

				if (get_hei() / zoom < LevelGridCountH * Config.GRID)
					root.Y = (float)(-position.Y * zoom + get_hei() * 0.5);
				else
					root.Y = (float)(get_hei() * 0.5 / zoom - LevelGridCountH * 0.5 * Config.GRID);

				// Clamp
				float pad = Config.GRID * 2;
				if (get_wid() < LevelGridCountW * Config.GRID * zoom)
					root.X = fclamp(root.X, get_wid() - LevelGridCountW * Config.GRID * zoom + pad, -pad);
				if (get_hei() < LevelGridCountH * Config.GRID * zoom)
					root.Y = fclamp(root.Y, get_hei() - LevelGridCountH * Config.GRID * zoom + pad, -pad);

				// Bumps friction
				bumpOffX *= (float)Math.Pow(0.75, tmod);
				bumpOffY *= (float)Math.Pow(0.75, tmod);

				// Bump
				root.X += bumpOffX;
				root.Y += bumpOffY;

				// Shakes
				if (shake)
				{
					shakeStarted += (float)gameTime.ElapsedGameTime.TotalSeconds;
					root.X += (float)(Math.Cos(gameTime.TotalGameTime.TotalMilliseconds * 1.1) * 2.5 * shakePower * 0.5f);
					root.Y += (float)(Math.Sin(0.3 + gameTime.TotalGameTime.TotalMilliseconds * 1.7) * 2.5 * shakePower * 0.5f);

					if (shakeStarted > shakeDuration)
					{
						shake = false;
						shakeStarted = 0f;
						//Recenter();

					}
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
