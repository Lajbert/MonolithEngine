using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonolithEngine
{
    public class MultiSelectionImage : Image
    {

        private List<Option> options = new List<Option>();

        private Option selected;

        public MultiSelectionImage(Vector2 position = default, Rectangle sourceRectangle = default, float scale = 1f, float rotation = 0f, int depth = 1, Color color = default) : base(null, position, sourceRectangle, scale, rotation, depth, color)
        {

        }

        public void AddOption(string label, Texture2D texture)
        {
            Option option = new Option(texture, label);
            options.Add(option);
            if (selected == null)
            {
                SetSelected(option);
            }
        }

        private void SetSelected(Option option)
        {
            selected = option;
            ImageTexture = option.texture;
            SourceRectangle = new Rectangle(0, 0, ImageTexture.Width, ImageTexture.Height);
        }

        public string GetSelection()
        {
            return selected.label;
        }

        public void SetSelected(string label)
        {
            foreach (Option o in options)
            {
                if (o.label.Equals(label))
                {
                    SetSelected(o);
                    return;
                }
            }

            throw new Exception("Element not found: " + label);
        }

        public void Next()
        {
            SetSelected(options[GetNextIndex(options.IndexOf(selected) + 1)]);
        }

        public void Previous()
        {
            SetSelected(options[GetNextIndex(options.IndexOf(selected) - 1)]);
        }

        private int GetNextIndex(int index)
        {
            if (index < 0)
            {
                return options.Count - 1;
            } else if (index == options.Count)
            {
                return 0;
            }
            return index;
        }

        private class Option
        {
            public Texture2D texture;
            public string label;

            public Option (Texture2D texture, string label)
            {
                this.texture = texture;
                this.label = label;
            }
        }
    }
}
