using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonolithEngine
{
    /// <summary>
    /// Camera class abstraction.
    /// </summary>
    public class Camera
    {

        public enum CameraMode
        {
            SINGLE,
            DUAL_HORIZONTAL_SPLIT,
            DUAL_VERTICAL_SPLIT
        }

        private const float MinZoom = 0.01f;

        internal Viewport Viewport;
        private Vector2 origin;

        private Vector2 position;
        private float zoom = 1f;
        private Rectangle? limits;

        public Entity target;
        private Vector2 targetTracingOffset = Vector2.Zero;
        private float friction = 0.89f;

        private float shakePower = 1.5f;
        private float shakeStarted = 0f;
        private float shakeDuration = 0f;
        private bool easedStop;

        private bool SCROLL = true;

        private bool shake = false;

        private Vector2 direction;

        private Matrix uiTransofrmMatrix;

        private float scrollSpeedModifier;

        private GraphicsDeviceManager graphicsDeviceManager;

        private float rotation = MathUtil.DegreesToRad(0);

        private CameraMode cameraMode;

        private Vector2 moveToPosition = default;

        private int cameraNumber;
        
        public Camera(GraphicsDeviceManager graphicsDeviceManager, CameraMode cameraMode = CameraMode.SINGLE, int cameraNumber = 0)
        {
            this.graphicsDeviceManager = graphicsDeviceManager;
            Position = Vector2.Zero;
            direction = Vector2.Zero;
            Zoom = Config.SCALE;
            this.cameraMode = cameraMode;
            this.cameraNumber = cameraNumber;
            Initialize();
        }

        /// <summary>
        /// Initializing the camera based on the screen resolution.
        /// </summary>
        public void Initialize()
        {
            Viewport = graphicsDeviceManager.GraphicsDevice.Viewport;

            if (cameraMode != CameraMode.SINGLE)
            {
                if (cameraMode == CameraMode.DUAL_VERTICAL_SPLIT)
                {
                    Viewport.Width /= 2;
                    if (cameraNumber == 1)
                    {
                        Viewport.X += Viewport.Width;
                    }
                }
                else if (cameraMode == CameraMode.DUAL_HORIZONTAL_SPLIT)
                {
                    Viewport.Height /= 2;
                    if (cameraNumber == 1)
                    {
                        Viewport.Y += Viewport.Height;
                    }
                }
            }

            Logger.Info("Configuring camera, viewport: [" + Viewport.ToString() + "], mode [" + cameraMode + "], camera number: " + cameraNumber);

            origin = new Vector2(Viewport.Width / 2.0f, Viewport.Height / 2.0f);
            Zoom = Config.SCALE;
            if (target != null)
            {
                TrackTarget(target, true, targetTracingOffset);
            }
            uiTransofrmMatrix = Matrix.Identity * Matrix.CreateScale(Config.SCALE, Config.SCALE, 1);
        }

        /// <summary>
        /// Shakes the camera
        /// </summary>
        /// <param name="power"></param>
        /// <param name="duration"></param>
        /// <param name="easeOut"></param>
        public void Shake(float power = 5, float duration = 300, bool easeOut = true)
        {
            shakePower = power;
            shakeDuration = duration;
            shake = true;
            easedStop = easeOut;
        }

        /// <summary>
        /// Updating the camera. If there is a target entity, we have to follow it.
        /// </summary>
        public void Update()
        {
            if (!SCROLL)
            {
                return;
            }
            float zoomMultiplier = Math.Max(1, Zoom / 2);
            float deadzone = Config.CAMERA_DEADZONE / zoomMultiplier;
            float elapsedTime = (float)Globals.ElapsedTime / Config.CAMERA_TIME_MULTIPLIER;
            // Follow target entity
            if (target != null || moveToPosition != default)
            {
                Vector2 targetPosition = moveToPosition;
                if (target != null)
                {
                    targetPosition = target.Transform.Position + targetTracingOffset - new Vector2(Viewport.Width / 2.0f, Viewport.Height / 2.0f);
                }

                float targetCameraDistance = Vector2.Distance(Position, targetPosition);
                if (targetCameraDistance >= deadzone)
                {
                    float angle = MathUtil.RadFromVectors(Position, targetPosition);
                    direction.X += (float)Math.Cos(angle) * (targetCameraDistance - deadzone) * (Config.CAMERA_FOLLOW_DELAY / zoomMultiplier) * elapsedTime;
                    direction.Y += (float)Math.Sin(angle) * (targetCameraDistance - deadzone) * (Config.CAMERA_FOLLOW_DELAY / zoomMultiplier) * elapsedTime;
                }
            }

            Position += direction * elapsedTime * zoomMultiplier;

            direction *= new Vector2((float)Math.Pow(friction, elapsedTime), (float)Math.Pow(friction, elapsedTime));

            PostUpdate();
        }

        /// <summary>
        /// Let's shake it!
        /// </summary>
        private void PostUpdate()
        {
            // Shakes
            if (shake)
            {
                shakeStarted += Globals.ElapsedTime;
                float power = shakePower;
                if (easedStop)
                {
                    float alpha = shakeStarted / shakeDuration;
                    power = MathHelper.Lerp(shakePower, 0, alpha);
                }
                Position += new Vector2((float)(Math.Cos(Globals.GameTime.TotalGameTime.TotalMilliseconds * 1.1) * power), (float)(Math.Sin(0.3 + Globals.GameTime.TotalGameTime.TotalMilliseconds * 1.7) * power));

                if (shakeStarted > shakeDuration)
                {
                    shake = false;
                    shakeStarted = 0f;
                }
            }
        }

        public void MoveTo(Vector2 position)
        {
            moveToPosition = position;
        }

        public void MoveBy(Vector2 position)
        {
            moveToPosition += position;
        }

        public Vector2 ScreenToWorldSpace(Vector2 mousePosition)
        {
            return Vector2.Transform(mousePosition, Matrix.Invert(WorldTranformMatrix));
        }

        public void TrackTarget(Entity entity, bool immediate = false, Vector2 tracingOffset = default)
        {
            targetTracingOffset = tracingOffset;
            target = entity;
            if (immediate)
            {
                ImmediateRecenter();
            }
        }

        public void StopTracking()
        {
            target = null;
        }

        public void ImmediateRecenter()
        {
            if (target != null)
            {
                Position = target.Transform.Position + targetTracingOffset - new Vector2(Viewport.Width / 2.0f, Viewport.Height / 2.0f);
            }
        }

        /// <summary>
        /// We need to validate the camera's position and ZOOM whenever we
        /// change these values.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                ValidatePosition();
            }
        }

        public float Zoom
        {
            get
            {
                return zoom;
            }
            set
            {
                zoom = MathHelper.Max(value, MinZoom);
                ValidateZoom();
                ValidatePosition();
            }
        }

        public Rectangle? Limits
        {
            set
            {
                limits = value;
                ValidateZoom();
                ValidatePosition();
            }
        }

        /// <summary>
        /// The default ViewMatrix of the camera based on the position and zoom. 
        /// </summary>
        public Matrix WorldTranformMatrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(-new Vector2(position.X * scrollSpeedModifier, position.Y), 0f)) *
                       Matrix.CreateTranslation(new Vector3(-origin, 0f)) *
                       Matrix.CreateRotationZ(rotation) *
                       Matrix.CreateScale(zoom, zoom, 1f) *
                       Matrix.CreateTranslation(new Vector3(origin, 0f))
                       ;
            }
        }

        private void ValidatePosition()
        {
            if (limits.HasValue)
            {
                Vector2 cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(WorldTranformMatrix));
                Vector2 cameraSize = new Vector2(Viewport.Width, Viewport.Height) / zoom;
                Vector2 limitWorldMin = new Vector2(limits.Value.Left, limits.Value.Top);
                Vector2 limitWorldMax = new Vector2(limits.Value.Right, limits.Value.Bottom);
                Vector2 positionOffset = position - cameraWorldMin;
                position = Vector2.Clamp(cameraWorldMin, limitWorldMin, limitWorldMax - cameraSize) + positionOffset;
            }
        }

        private void ValidateZoom()
        {
            if (limits.HasValue)
            {
                float minZoomX = (float)Viewport.Width / limits.Value.Width;
                float minZoomY = (float)Viewport.Height / limits.Value.Height;
                zoom = MathHelper.Max(zoom, MathHelper.Max(minZoomX, minZoomY));
            }
        }

        /// <summary>
        /// A static transformation matrix for the UI. This The position and 
        /// zoom are fixed (we don't scroll and zoom in/out into UI elements)
        /// </summary>
        /// <returns></returns>
        public Matrix GetUITransformMatrix()
        {
            return uiTransofrmMatrix;
        }

        /// <summary>
        /// Getter for the transformation matrix. It takes scrollSpeedModifier
        /// into account for parallax scrolling.
        /// </summary>
        /// <param name="scrollSpeedModifier">The scroll speed modifier for parallax backgrounds</param>
        /// <param name="lockY">'true' if we want to lock the scrolling of the Y axis</param>
        /// <returns></returns>
        public Matrix GetWorldTransformMatrix(float scrollSpeedModifier = 1f, bool lockY = false)
        {
            this.scrollSpeedModifier = scrollSpeedModifier;
            return WorldTranformMatrix;
        }
    }
}
