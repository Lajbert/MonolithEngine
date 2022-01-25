using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using MonolithEngine;

namespace MonolithEngine
{
    public class GameButton : SelectableImage
    {
        private float defaultScale;
        private float selectedScale;
        private Vector2 defaultOrigin;
        private float wubblyScaleOffset;
        public float ScaleEffectAmount = 0.3f;
        public bool IsAnimated = true;

        public GameButton(Texture2D texture, Vector2 position, float scale, IUIElement parent = null, bool useTextureSizeOffset = true) : base(texture, texture, parent, position, scale: scale)
        {
            defaultScale = scale;
            selectedScale = defaultScale * 1.5f;
            defaultOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
            Origin = defaultOrigin;
            OwnPosition += Origin;
            if (useTextureSizeOffset)
            {
                OwnPosition -= new Vector2(texture.Width / 2, texture.Height / 2);
            }
            SelectionBoxOffset = -Origin * defaultScale;
            OnResolutionChanged();

            if (MonolithGame.Platform.IsMobile())
            {
                ScaleEffectAmount /= 10f;
            }
        }

        public override void Update(TouchCollection touchLocations)
        {
            AdjustSprite();
            base.Update(touchLocations);
        }

        public override void Update(Point mousePosition = default)
        {
            AdjustSprite();
            base.Update(mousePosition);
        }

        private void AdjustSprite()
        {
            if (!IsAnimated)
            {
                return;
            }
            if (MonolithGame.Platform.IsDesktop())
            {
                if (IsSelected || IsHoveredOver)
                {
                    if (Scale != selectedScale)
                    {
                        Scale = selectedScale;
                        SelectionBoxOffset = -Origin * selectedScale;
                    }
                }
                else
                {
                    if (Scale != defaultScale)
                    {
                        Scale = defaultScale;
                        SelectionBoxOffset = -Origin * defaultScale;
                    }
                }
            }
            Scale += (float)Math.Sin(MathUtil.DegreesToRad(wubblyScaleOffset)) * ScaleEffectAmount;
            OnResolutionChanged();
            wubblyScaleOffset += Globals.ElapsedTime * 0.1f;
            if (wubblyScaleOffset >= 360)
            {
                wubblyScaleOffset = 0;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
