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

        // Calculated Vector2 for center of sprite
        public Vector2 Center
        {
            get
            {
                return new Vector2(Position.X + (Width / 2), Position.Y + (Height / 2));
            }
        }

        public bool IsAlive { get; set; }

        // Rectangle set at size of texture
        public Rectangle BoundingRect
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
            }
        }

        // Create sprite with texture at position
        // Calculate sprite dimensions based on scale
        // set to alive on creation
        public Sprite(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            Position = position;
            Height = texture.Height * (int)Game1.Scale.Y;
            Width = texture.Width * (int)Game1.Scale.X;
            IsAlive = true;
            
        }

        // Create sprite with texture
        // Calculate sprite dimensions based on scale
        // set to alive on creation
        // sprite will have no position and will not be drawn
        public Sprite(Texture2D texture)
        {
            this.texture = texture;
            Position = Vector2.Zero;
            Width = texture.Width * (int)Game1.Scale.X;
            Height = texture.Height * (int)Game1.Scale.Y;
            IsAlive = true;
        }

        // Draw sprite at position at Game1.Scale
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

        // Draw sprite at position, tint with color
        public virtual void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(
                texture,
                new Vector2((int)Position.X, (int)Position.Y),
                null,
                color,
                0.0f,
                Vector2.Zero,
                Game1.Scale,
                SpriteEffects.None,
                0.0f);
        }
        
    }
}
