using GameEngine2D.Entities;
using GameEngine2D.Global;
using GameEngine2D.Source.Util;
using GameEngine2D.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Source.Camera2D
{
	public class Camera
	{
		public static Entity target { get; set; }

		public static Vector2 Position;
		//public float x;
		//public float y;

		public Vector2 direction;
		//public float dx;
		//public float dy;
		//public int wid;
		//public int hei;

		private float bumpOffX = 0f;
		private float bumpOffY = 0f;

		public float Zoom = 1f;

		private float resolutionWidth = Config.RES_W;
		private float resolutionHeight = Config.RES_H;
		public float LevelGridCountW = (float)Math.Floor((decimal)Config.RES_W);
		public float LevelGridCountH = (float)Math.Floor((decimal)Config.RES_H);

		private float shakePower = 1.5f;
		private float shakeStarted = 0f;
		private float shakeDuration = 0f;

		public float Scale = 1f;
		private bool SCROLL = false;

		private bool shake = false;

		private float elapsedTime;

		private Vector2 targetPosition = Vector2.Zero;
		private Vector2 targetTracingOffset = Vector2.Zero;
		private float targetCameraDistance;
		private float a;

		float frict = 0.89f;

		RootContainer root;

		private Matrix transformMatrix;

		private Vector2 viewportCenter;

		public Camera(GraphicsDeviceManager graphicsDeviceManager) {
			//x = y = 0;
			//dx = dy = 0;
			Position = Vector2.Zero;
			direction = Vector2.Zero;
			resolutionWidth = graphicsDeviceManager.PreferredBackBufferWidth;
			resolutionHeight = graphicsDeviceManager.PreferredBackBufferHeight;
			viewportCenter = new Vector2(graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight) / 2;
		}

		private float get_wid()
		{
			return (float)Math.Ceiling(resolutionWidth / Scale);
		}

		private float get_hei()
		{
			return (float)Math.Ceiling(resolutionHeight / Scale);
		}

		public void trackTarget(Entity e, bool immediate, Vector2 tracingOffset = new Vector2())
		{
			targetTracingOffset = tracingOffset;
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
				Position = target.Position + targetTracingOffset;
			}
		}

		public float scrollerToGlobalX(float v) {
			return v * Scale + RootContainer.Instance.Position.X;
		}
		public float scrollerToGlobalY(float v) {
			return v * Scale + RootContainer.Instance.Position.Y;
		}

		public void Shake(float power = 1, float duration = 1)
        {
			shakePower = power;
			shakeDuration = duration;
			shake = true;
        }

		public void update(GameTime gameTime)
		{
			elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds / Config.CAMERA_TIME_MULTIPLIER;
			// Follow target entity
			if (target != null)
			{
				targetPosition = target.Position + targetTracingOffset;

				targetCameraDistance = MathUtil.Distance(Position, targetPosition);
				if (targetCameraDistance >= Config.CAMERA_DEADZONE)
				{
					a = (float)Math.Atan2(targetPosition.Y - Position.Y, targetPosition.X - Position.X);
					direction.X += (float)Math.Cos(a) * (targetCameraDistance - Config.CAMERA_DEADZONE) * Config.CAMERA_FOLLOW_DELAY * elapsedTime;
					direction.Y += (float)Math.Sin(a) * (targetCameraDistance - Config.CAMERA_DEADZONE) * Config.CAMERA_FOLLOW_DELAY * elapsedTime;
				}
			}

			Position.X += direction.X * elapsedTime;
			direction.X *= (float)Math.Pow(frict, elapsedTime);

			Position.Y += direction.Y * elapsedTime;
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
			elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			if (SCROLL)
			{
				root = RootContainer.Instance;

				// Update scroller
				if (get_wid() / Zoom < LevelGridCountW * Config.GRID)
					root.X = (float)(-Position.X * Zoom + get_wid() * 0.5);
				else
					root.X = (float)(get_wid() * 0.5 / Zoom - LevelGridCountW * 0.5 * Config.GRID);

				if (get_hei() / Zoom < LevelGridCountH * Config.GRID)
					root.Y = (float)(-Position.Y * Zoom + get_hei() * 0.5);
				else
					root.Y = (float)(get_hei() * 0.5 / Zoom - LevelGridCountH * 0.5 * Config.GRID);

				// Clamp
				float pad = Config.GRID * 2;
				if (get_wid() < LevelGridCountW * Config.GRID * Zoom)
					root.X = MathUtil.Clamp(root.X, get_wid() - LevelGridCountW * Config.GRID * Zoom + pad, -pad);
				if (get_hei() < LevelGridCountH * Config.GRID * Zoom)
					root.Y = MathUtil.Clamp(root.Y, get_hei() - LevelGridCountH * Config.GRID * Zoom + pad, -pad);

				// Bumps friction
				bumpOffX *= (float)Math.Pow(0.75, elapsedTime);
				bumpOffY *= (float)Math.Pow(0.75, elapsedTime);

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
				root.X *= Scale;
				root.Y *= Scale;

				// Rounding
				root.X = (float)Math.Round(root.X);
				root.Y = (float)Math.Round(root.Y);

				// Zoom
				//scroller.setScale(SCALE * zoom);
			}
		}

		public Matrix GetTransformMatrix()
        {
			CalculateTransformMatrix();

			return transformMatrix;
        }
		
		private void CalculateTransformMatrix()
        {
			transformMatrix =
				Matrix.CreateTranslation(new Vector3(-Position + viewportCenter, 0)) *
				Matrix.CreateTranslation(new Vector3(-viewportCenter, 0)) *
				Matrix.CreateScale(Config.ZOOM, Config.ZOOM, 1) *
				Matrix.CreateTranslation(new Vector3(viewportCenter, 0));
		}
	}
}
