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
        public static int WIDTH = 16 * (int)Game1.Scale.X;
        public static int HEIGHT = 16 * (int)Game1.Scale.Y;

        public Rectangle SourceRectangle { get; private set; }
        public Vector2 Position { get; private set; }
        public bool IsSolid { get; private set; }

        public Rectangle BoundingRect
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, WIDTH, HEIGHT);
            }
        }


        public Tile(Rectangle sourceRectangle, Vector2 position, bool isSolid)
        {
            SourceRectangle = sourceRectangle;
            Position = position;
            IsSolid = isSolid;
        }
    }
}
