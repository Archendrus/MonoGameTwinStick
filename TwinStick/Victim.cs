using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStick
{
    class Victim : Sprite
    {

        // Rectangle for collision based on non-transparent area of sprite
        public Rectangle CollisionRect
        {
            get
            {
                return new Rectangle(
                    (int)Position.X + (4 * (int)scale.X),
                    (int)Position.Y,
                    (8 * (int)scale.X),
                    Height);
            }
        }

        // Create victim with texture, at position, at using scale
        public Victim(Texture2D texture, Vector2 position, Vector2 scale)
            : base(texture, position, scale)
        {

        }

        // Create victim with texutre, using scale
        // (Set position after creation)
        public Victim(Texture2D texture, Vector2 scale)
            : base(texture, scale)
        {

        }
    }
}
