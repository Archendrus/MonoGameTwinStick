using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStick
{  
    class Tile
    {
        public static int Width { get; private set; }
        public static int Height { get; private set; }

        // Source rectangle from tile sheet
        public Rectangle SourceRectangle { get; private set; }

        public Vector2 Position { get; private set; }
        public bool IsSolid { get; private set; }

        Vector2 scale;

        // Rectangle set at size of texture
        public Rectangle BoundingRect
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        // Create new tile using texture at sourceRectangle of tile sheet,
        // set at position, set solid state to isSolid
        public Tile(Rectangle sourceRectangle, Vector2 position, Vector2 scale, bool isSolid)
        {
            SourceRectangle = sourceRectangle;
            Position = position;
            IsSolid = isSolid;
            this.scale = scale;

            Width = 16 * (int)scale.X;
            Height = 16 * (int)scale.Y;
        }
    }
}
