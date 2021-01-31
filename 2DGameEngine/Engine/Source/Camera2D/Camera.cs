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

		private static Vector2 position = Vector2.Zero;

		private Vector2 direction;

		private Vector2 bumpOffset;

		private float resolutionWidth = Config.RES_W;
		private float resolutionHeight = Config.RES_H;
		public float LevelGridCountW = (float)Math.Floor((decimal)Config.RES_W);
		public float LevelGridCountH = (float)Math.Floor((decimal)Config.RES_H);
		//public float LevelGridCountW = 98;
		//public float LevelGridCountH = 36;

		private float shakePower = 1.5f;
		private float shakeStarted = 0f;
		private float shakeDuration = 0f;

		private bool SCROLL = true;

		private bool shake = false;

		private float elapsedTime;

		private Vector2 targetPosition = Vector2.Zero;
		private Vector2 targetTracingOffset = Vector2.Zero;
		private float targetCameraDistance;
		private float angle;

		private float friction = 0.89f;

		private Matrix transformMatrix;

		private Vector2 viewportCenter;
		private Vector3 viewportCenterTransform;
		public Vector2 CurrentCenter;

		public int BOUND_LEFT = 500;
        public int BOUND_RIGHT = 2000;
		public int BOUND_TOP = 350;
		public int BOUND_BOTTOM = 450;

		private LayerManager root;

		public Camera(GraphicsDeviceManager graphicsDeviceManager) {
			position = Vector2.Zero;
			direction = Vector2.Zero;
			resolutionWidth = graphicsDeviceManager.PreferredBackBufferWidth;
			resolutionHeight = graphicsDeviceManager.PreferredBackBufferHeight;
			viewportCenter = new Vector2(graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight) / 2;
			viewportCenterTransform = new Vector3(viewportCenter, 0);
		}

		public void TrackTarget(Entity e, bool immediate, Vector2 tracingOffset = new Vector2())
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
				position = target.Position + targetTracingOffset;
			}
		}

		public void Shake(float power = 1, float duration = 1)
        {
			shakePower = power;
			shakeDuration = duration;
			shake = true;
        }

		public void update(GameTime gameTime)
		{
			if (!SCROLL)
            {
				return;
            }
			elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds / Config.CAMERA_TIME_MULTIPLIER;
			// Follow target entity
			if (target != null)
			{
				targetPosition = target.Position + targetTracingOffset;

				targetCameraDistance = Vector2.Distance(position, targetPosition);
				if (targetCameraDistance >= Config.CAMERA_DEADZONE)
				{
					angle = MathUtil.RadFromVectors(position, targetPosition);
					direction.X += (float)Math.Cos(angle) * (targetCameraDistance - Config.CAMERA_DEADZONE) * Config.CAMERA_FOLLOW_DELAY * elapsedTime;
					direction.Y += (float)Math.Sin(angle) * (targetCameraDistance - Config.CAMERA_DEADZONE) * Config.CAMERA_FOLLOW_DELAY * elapsedTime;
				}
			}

			position += direction * elapsedTime;

			if (position.X < BOUND_LEFT)
			{
				position.X = BOUND_LEFT;
			}
			if (position.Y < BOUND_TOP)
			{
				position.Y = BOUND_TOP;
			}
			if (position.X > BOUND_RIGHT)
			{
				position.X = BOUND_RIGHT;
			}
			if (position.Y > BOUND_BOTTOM)
			{
				position.Y = BOUND_BOTTOM;
			}
			direction *= new Vector2((float)Math.Pow(friction, elapsedTime), (float)Math.Pow(friction, elapsedTime));
		}

		public void postUpdate(GameTime gameTime)
		{
			/*elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			if (SCROLL)
			{
				root = RootContainer.Instance;

				// Update scroller
				if (resolutionWidth < LevelGridCountW * Config.GRID)
					root.X = (float)(-position.X + resolutionWidth * 0.5);
				else
					root.X = (float)(resolutionWidth * 0.5  - LevelGridCountW * 0.5 * Config.GRID);

				if (resolutionHeight < LevelGridCountH * Config.GRID)
					root.Y = (float)(-position.Y + resolutionHeight * 0.5);
				else
					root.Y = (float)(resolutionHeight * 0.5 - LevelGridCountH * 0.5 * Config.GRID);

				// Clamp
				float pad = Config.GRID * 2;
				if (resolutionWidth < LevelGridCountW * Config.GRID)
					root.X = MathUtil.Clamp(root.X, resolutionWidth - LevelGridCountW * Config.GRID + pad, -pad);
				if (resolutionHeight < LevelGridCountH * Config.GRID)
					root.Y = MathUtil.Clamp(root.Y, resolutionHeight - LevelGridCountH * Config.GRID + pad, -pad);

				// Bumps friction
				bumpOffset *= new Vector2((float)Math.Pow(0.75, elapsedTime), (float)Math.Pow(0.75, elapsedTime));

				// Bump
				root.Position += bumpOffset;
			
				// Shakes
				if (shake)
				{
					shakeStarted += (float)gameTime.ElapsedGameTime.TotalSeconds;
					position.X += (float)(Math.Cos(gameTime.TotalGameTime.TotalMilliseconds * 1.1) * 2.5 * shakePower * 0.5f);
					position.Y += (float)(Math.Sin(0.3 + gameTime.TotalGameTime.TotalMilliseconds * 1.7) * 2.5 * shakePower * 0.5f);

					if (shakeStarted > shakeDuration)
					{
						shake = false;
						shakeStarted = 0f;
						//Recenter();

					}
				}

				// Rounding
				root.Position = MathUtil.Round(root.Position);
			}*/
		}

		public Matrix GetTransformMatrix(float scrollSpeedModifier = 1f, bool lockY = false)
        {
			CalculateTransformMatrix(scrollSpeedModifier, lockY);

			return transformMatrix;
        }
		
		private void CalculateTransformMatrix(float scrollSpeedModifier = 1f, bool lockY = true)
        {
			if (lockY)
            {
				transformMatrix =
					Matrix.CreateTranslation(new Vector3(new Vector2((-position.X + viewportCenter.X) * scrollSpeedModifier, (-position.Y + viewportCenter.Y)), 0)) *
					Matrix.CreateTranslation(-viewportCenterTransform) *
					Matrix.CreateScale(Config.ZOOM, Config.ZOOM, 1) *
					Matrix.CreateTranslation(viewportCenterTransform);
			} else
            {
				transformMatrix =
					Matrix.CreateTranslation(new Vector3(-position + viewportCenter, 0) * scrollSpeedModifier) *
					Matrix.CreateTranslation(-viewportCenterTransform) *
					Matrix.CreateScale(Config.ZOOM, Config.ZOOM, 1) *
					Matrix.CreateTranslation(viewportCenterTransform);
			}
		}
	}
}
