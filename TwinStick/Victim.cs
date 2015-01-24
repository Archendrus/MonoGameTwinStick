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
        public Rectangle CollisionRect
        {
            get
            {
                return new Rectangle(
                    (int)Position.X + (4 * (int)Game1.Scale.X),
                    (int)Position.Y,
                    (8 * (int)Game1.Scale.X),
                    Height);
            }
        }

        public Victim(Texture2D texture, Vector2 position)
            : base(texture, position)
        {

        }

        public Victim(Texture2D texture)
            : base(texture)
        {

        }
    }
}
