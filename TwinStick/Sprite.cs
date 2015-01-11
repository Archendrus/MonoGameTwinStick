using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStick
{
    class Sprite
    {
        private Texture2D texture;

        public Vector2 Position { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Rectangle BoundingRect
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        public Sprite(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            Position = position;
            Height = texture.Height * (int)Game1.Scale.Y;
            Width = texture.Width * (int)Game1.Scale.X;
            
        }

        public Sprite(Texture2D texture)
        {
            this.texture = texture;
            Position = Vector2.Zero;
            Width = texture.Width * (int)Game1.Scale.X;
            Height = texture.Height * (int)Game1.Scale.Y;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                new Vector2((int)Position.X, (int)Position.Y),
                null,
                Color.White,
                0.0f,
                Vector2.Zero,
                Game1.Scale,
                SpriteEffects.None,
                0.0f);
        }
        
    }
}
