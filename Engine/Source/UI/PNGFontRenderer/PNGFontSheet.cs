using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonolithEngine
{
    public class PNGFontSheet
    {

        private Dictionary<char, Rectangle> charMappings = new Dictionary<char, Rectangle>();
        public Texture2D FontSheet;

        public PNGFontSheet(Texture2D fontSheet, char[,] keyMapping, Vector2 grid = default)
        {
            if (grid == default)
            {
                throw new Exception("Automatic grid detection for font sheet is not implemented yet!");
            }

            FontSheet = fontSheet;

            int gridWidth = 8;
            int gridHeight = 8;

            for (int i = 0; i < gridWidth; i++)
            {
                for (int j = 0; j < gridHeight; j++)
                {
                    Rectangle srcRect = new Rectangle((int)(i * grid.X), (int)(j * grid.Y), (int)grid.X - 1, (int)grid.Y - 1);
                    Texture2D letter = AssetUtil.GetSubTexture(fontSheet, srcRect);
                    Rectangle autoBB = AssetUtil.AutoBoundingBox(letter);
                    charMappings[keyMapping[j, i]] = new Rectangle(srcRect.X + autoBB.X, srcRect.Y + autoBB.Y, autoBB.Width, autoBB.Height);
                }
            }
        }

        public Rectangle GetSourceRectangle(char c)
        {
            return charMappings[c];
        }
    }
}
