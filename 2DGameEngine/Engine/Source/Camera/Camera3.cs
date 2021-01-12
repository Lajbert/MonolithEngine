using GameEngine2D.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine2D.Engine.Source.Camera
{
    public class Camera3
    {
        public Camera3(Viewport viewport)
        {
            _viewport = viewport;
            _origin = new Vector2(_viewport.Width / 2.0f, _viewport.Height / 2.0f);
        }

        /// <summary>
        /// Gets or sets the position of the camera.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                ValidatePosition();
            }
        }

        /// <summary>
        /// Gets or sets the zoom of the camera.
        /// </summary>
        public float Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = MathHelper.Max(value, MinZoom);
                ValidateZoom();
                ValidatePosition();
            }
        }

        /// <summary>
        /// Sets a rectangle that describes which region of the world the camera should
        /// be able to see. Setting it to null removes the limit.
        /// </summary>
        public Rectangle? Limits
        {
            set
            {
                _limits = value;
                ValidateZoom();
                ValidatePosition();
            }
        }

        /// <summary>
        /// Calculates a view matrix for this camera.
        /// </summary>
        public Matrix ViewMatrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-_position, 0f)) *
                       Matrix.CreateTranslation(new Vector3(-_origin, 0f)) *
                       Matrix.CreateScale(Config.SCALE, Config.SCALE, 1f) *
                       Matrix.CreateTranslation(new Vector3(_origin, 0f));
            }
        }

        /// <summary>
        /// When using limiting, makes sure the camera position is valid.
        /// </summary>
        private void ValidatePosition()
        {
            if (_limits.HasValue)
            {
                Vector2 cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(ViewMatrix));
                Vector2 cameraSize = new Vector2(_viewport.Width, _viewport.Height) / _zoom;
                Vector2 limitWorldMin = new Vector2(_limits.Value.Left, _limits.Value.Top);
                Vector2 limitWorldMax = new Vector2(_limits.Value.Right, _limits.Value.Bottom);
                Vector2 positionOffset = _position - cameraWorldMin;
                _position = Vector2.Clamp(cameraWorldMin, limitWorldMin, limitWorldMax - cameraSize) + positionOffset;
            }
        }

        /// <summary>
        /// When using limiting, makes sure the camera zoom is valid.
        /// </summary>
        private void ValidateZoom()
        {
            if (_limits.HasValue)
            {
                float minZoomX = (float)_viewport.Width / _limits.Value.Width;
                float minZoomY = (float)_viewport.Height / _limits.Value.Height;
                _zoom = MathHelper.Max(_zoom, MathHelper.Max(minZoomX, minZoomY));
            }
        }

        private const float MinZoom = 0.01f;

        private readonly Viewport _viewport;
        private readonly Vector2 _origin;

        private Vector2 _position;
        private float _zoom = 1f;
        private Rectangle? _limits;
    }
}
