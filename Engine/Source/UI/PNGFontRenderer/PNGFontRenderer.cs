using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine
{
    public class PNGFontRenderer : AbstractUIElement
    {
        private PNGFontSheet fontSheet;
        private string toRender = null;
        private Func<string> toRenderFunc = null;
        public float Scale = 1f;
        public Vector2 LetterSpacingOffset = Vector2.Zero;
        public Vector2 PositionOffsetPixels = Vector2.Zero;
        private string renderedSoFar = "";
        public bool AnimatedDisplay = false;

        public PNGFontRenderer(PNGFontSheet fontSheet, string toRender, Vector2 position, IUIElement parent = null) : base(position, parent)
        {
            this.fontSheet = fontSheet;
            this.toRender = toRender.ToLower();
        }

        public PNGFontRenderer(PNGFontSheet fontSheet, Func<string> toRender, Vector2 position, IUIElement parent = null) : base(position, parent)
        {
            this.fontSheet = fontSheet;
            this.toRenderFunc = toRender;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (toRenderFunc != null)
            {
                toRender = toRenderFunc.Invoke();
            }
            
            if (AnimatedDisplay)
            {
                if (!Timer.IsSet("TextRenderer") && !Timer.IsSet("TextRenderFinished"))
                {
                    int nextChar = renderedSoFar.Length;
                    renderedSoFar += toRender.ToCharArray()[nextChar].ToString();
                    Timer.SetTimer("TextRenderer", 200);
                }
                if (renderedSoFar.Equals(toRender) && !Timer.IsSet("TextRenderFinished"))
                {
                    Timer.SetTimer("TextRenderFinished", 1000);
                    Timer.TriggerAfter(1000, () =>
                    {
                        renderedSoFar = "";
                    });
                }
            }

            Vector2 charPos = Vector2.Zero;
            string text = AnimatedDisplay ? renderedSoFar : toRender;
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    charPos = new Vector2(0, charPos.Y + (fontSheet.GetSourceRectangle('a').Height + LetterSpacingOffset.Y) * Scale);
                }
                else if (c == ' ')
                {
                    charPos += new Vector2((fontSheet.GetSourceRectangle('a').Width + LetterSpacingOffset.X) * Scale, 0);
                }
                else
                {
                    Vector2 toRenderPos = GetPosition() + charPos;
                    spriteBatch.Draw(fontSheet.FontSheet, toRenderPos + PositionOffsetPixels, fontSheet.GetSourceRectangle(c), Color.White, 0f, Origin, Scale, SpriteEffects.None, 0);
                    charPos += new Vector2((fontSheet.GetSourceRectangle(c).Width + LetterSpacingOffset.X) * Scale , 0);
                }
            }
        }

        public Vector2 GetTextDimensions()
        {
            Vector2 result = Vector2.Zero;

            foreach (char c in toRender)
            {
                if (c == '\n')
                {
                    result += new Vector2(0, (fontSheet.GetSourceRectangle('a').Height + LetterSpacingOffset.Y) * Scale);
                }
                else if (c == ' ')
                {
                    result += new Vector2((fontSheet.GetSourceRectangle('a').Width + LetterSpacingOffset.X) * Scale, 0);
                }
                else
                {
                    Rectangle rect = fontSheet.GetSourceRectangle(c);
                    if (result == Vector2.Zero)
                    {

                        result += new Vector2((fontSheet.GetSourceRectangle(c).Width + LetterSpacingOffset.X) * Scale, (fontSheet.GetSourceRectangle(c).Height + LetterSpacingOffset.Y) * Scale);
                    }
                    else
                    {
                        result += new Vector2((fontSheet.GetSourceRectangle(c).Width + LetterSpacingOffset.X) * Scale, 0);
                    }
                }
            }
            return result;
        }
    }
}
