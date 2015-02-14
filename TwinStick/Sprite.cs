using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStick
{
    // Base sprite class
    // Has a texture position, width, and height
    // IsAlive flag if active
    // Multiple draw methods
    class Sprite
    {
        private Texture2D texture;

        public Vector2 Position { get; set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        protected Vector2 scale;

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
        public Sprite(Texture2D texture, Vector2 position, Vector2 scale)
        {
            this.texture = texture;
            this.scale = scale; 
            Position = position;

            // Calculate actual sprite size
            Height = texture.Height * (int)scale.Y;
            Width = texture.Width * (int)scale.X;

            IsAlive = true;          
        }

        // Create sprite with texture
        // Calculate sprite dimensions based on scale
        // set to alive on creation
        // sprite will have no position and will not be drawn
        public Sprite(Texture2D texture, Vector2 scale)
        {
            this.texture = texture;
            this.scale = scale;
            Position = Vector2.Zero;
            Width = texture.Width * (int)scale.X;
            Height = texture.Height * (int)scale.Y;
            IsAlive = true;
        }

        // Draw sprite at position at Game1.Scale
        public void Draw(SpriteBatch spriteBatch)
        {
            // Only draw if Sprite.IsAlive
            if (IsAlive)
            {
                spriteBatch.Draw(
                    texture,
                    new Vector2((int)Position.X, (int)Position.Y),
                    null,
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0.0f);
            }

        }

        // Draw sprite at position, tint with color
        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            // Only draw if Sprite.IsAlive
            if (IsAlive)
            {
                spriteBatch.Draw(
                    texture,
                    new Vector2((int)Position.X, (int)Position.Y),
                    null,
                    color,
                    0.0f,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0.0f);
            }
            
        }

        // Draw sprite at position with alpha
        public void Draw(SpriteBatch spriteBatch, float alpha)
        {
            // Only draw if Sprite.IsAlive
            if (IsAlive)
            {
                spriteBatch.Draw(
                    texture,
                    new Vector2((int)Position.X, (int)Position.Y),
                    null,
                    Color.White * alpha,
                    0.0f,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0.0f);
            }
        }
        
    }
}
