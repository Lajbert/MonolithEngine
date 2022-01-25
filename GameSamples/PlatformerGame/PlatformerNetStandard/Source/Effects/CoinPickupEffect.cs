using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using MonolithEngine;

namespace ForestPlatformerExample
{
    public class CoinPickupEffect : Image
    {
        private readonly Vector2 TARGET = new Vector2(5, 5);
        private readonly float DURATION_MS = 500;
        private AbstractScene scene;
        private Queue<(Vector2, float)> startPositions = new Queue<(Vector2, float)>();
        private static float DEFAULT_SCALE = 0.5f;

        public CoinPickupEffect(AbstractScene scene) : base(Assets.GetTexture2D("CoinEffect"), null,  Vector2.Zero, default, DEFAULT_SCALE)
        {
            if (MonolithGame.Platform.IsMobile())
            {
                DEFAULT_SCALE = 0.75f;
            }
            Scale = DEFAULT_SCALE;
            scene.UI.AddUIElement(this);
            this.scene = scene;
            Logger.Warn(" ### Coin pickup effect is still implemented as UI element, create proper particle system and replace it! ###");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (startPositions.Count == 0)
            {
                return;
            }
            for (int i = 0; i < startPositions.Count; i++)
            {
                (Vector2 startPosition, float elapsedTime) current = startPositions.Dequeue();
                if (current.elapsedTime >= DURATION_MS)
                {
                    PlatformerGame.CoinCount++;
                    return;
                }
                Vector2 lerpPos = Vector2.Lerp(current.startPosition, TARGET, current.elapsedTime / DURATION_MS);
                Scale = MathHelper.Lerp(DEFAULT_SCALE, DEFAULT_SCALE / 2, current.elapsedTime / DURATION_MS);
                current.elapsedTime += Globals.ElapsedTime;
                spriteBatch.Draw(ImageTexture, lerpPos, SourceRectangle, Color, Rotation, Vector2.Zero, Scale * scene.Cameras[0].Zoom, SpriteEffect, Depth);
                startPositions.Enqueue(current);
            }
        }

        public void AddCoin(Vector2 position)
        {
            startPositions.Enqueue((scene.Cameras[0].WorldToScreenSpace(position), 0f));
        }
    }
}
