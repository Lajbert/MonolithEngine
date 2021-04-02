using MonolithEngine.Engine.Source.Global;
using MonolithEngine.Entities;
using MonolithEngine.Global;
using MonolithEngine.Source.Util;
using MonolithEngine.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using MonolithEngine.Engine.Source.MyGame;

namespace MonolithEngine.Source.Camera2D
{
	public class Camera
	{
		public static Entity target { get; set; }

		private Vector2 position = Vector2.Zero;

		private Vector2 direction;

		private Vector2 bumpOffset;

		private float resolutionWidth = VideoConfiguration.RESOLUTION_WIDTH;
		private float resolutionHeight = VideoConfiguration.RESOLUTION_HEIGHT;
		public float LevelGridCountW = (float)Math.Floor((decimal)VideoConfiguration.RESOLUTION_WIDTH);
		public float LevelGridCountH = (float)Math.Floor((decimal)VideoConfiguration.RESOLUTION_HEIGHT);
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

		public int BOUND_LEFT = 0;
        public int BOUND_RIGHT = 0;
		public int BOUND_TOP = 0;
		public int BOUND_BOTTOM = 0;

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

		public void StopTracking()
		{
			target = null;
		}

		public void Recenter()
		{
			if (target != null) {
				position = target.Transform.Position + targetTracingOffset;
			}
		}

		public void Shake(float power = 1, float duration = 1)
        {
			shakePower = power;
			shakeDuration = duration;
			shake = true;
        }

		public void Update()
		{
			if (!SCROLL)
            {
				return;
            }

			elapsedTime = (float)Globals.ElapsedTime / Config.CAMERA_TIME_MULTIPLIER;
			// Follow target entity
			if (target != null)
			{
				targetPosition = target.Transform.Position + targetTracingOffset;

				targetCameraDistance = Vector2.Distance(position, targetPosition);
				if (targetCameraDistance >= Config.CAMERA_DEADZONE)
				{
					angle = MathUtil.RadFromVectors(position, targetPosition);
					direction.X += (float)Math.Cos(angle) * (targetCameraDistance - Config.CAMERA_DEADZONE) * Config.CAMERA_FOLLOW_DELAY * elapsedTime;
					direction.Y += (float)Math.Sin(angle) * (targetCameraDistance - Config.CAMERA_DEADZONE) * Config.CAMERA_FOLLOW_DELAY * elapsedTime;
				}
			}

			position += direction * elapsedTime;

			if (BOUND_LEFT != 0 && position.X < BOUND_LEFT)
			{
				position.X = BOUND_LEFT;
			}
			if (BOUND_TOP != 0 && position.Y < BOUND_TOP)
			{
				position.Y = BOUND_TOP;
			}
			if (BOUND_RIGHT != 0 && position.X > BOUND_RIGHT)
			{
				position.X = BOUND_RIGHT;
			}
			if (BOUND_BOTTOM != 0 && position.Y > BOUND_BOTTOM)
			{
				position.Y = BOUND_BOTTOM;
			}
			direction *= new Vector2((float)Math.Pow(friction, elapsedTime), (float)Math.Pow(friction, elapsedTime));

			PostUpdate();
		}

		private void PostUpdate()
		{
			// Shakes
			if (shake)
			{
				shakeStarted += Globals.ElapsedTime;
				position.X += (float)(Math.Cos(Globals.GameTime.TotalGameTime.TotalMilliseconds * 1.1) * 2.5 * shakePower * 0.5f);
				position.Y += (float)(Math.Sin(0.3 + Globals.GameTime.TotalGameTime.TotalMilliseconds * 1.7) * 2.5 * shakePower * 0.5f);

				if (shakeStarted > shakeDuration)
				{
					shake = false;
					shakeStarted = 0f;
				}
			}
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
