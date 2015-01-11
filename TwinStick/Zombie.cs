using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStick
{
    class Zombie : Sprite
    {
        float speed;
        public Rectangle CollisionRect
        {
            get
            {
                return new Rectangle((int)Position.X + 4, (int)Position.Y, 8, Height);
            }
        }

        public Circle CollisionCircle
        {
            get
            {
                return new Circle(
                    new Vector2(Position.X + (Width / 2),
                    Position.Y + (Height / 2)),
                    8.0f * Game1.Scale.X);
            }
        }

        public Zombie(Texture2D texture, Vector2 position)
            : base(texture, position)
        {
            speed = 35f;
        }

        public Zombie(Texture2D texture)
            : base(texture)
        {
            speed = 35;
        }

        public void Update(GameTime time, TileMap map, Vector2 playerPosition)
        {
            float elapsed = (float)time.ElapsedGameTime.TotalSeconds;
            Vector2 direction = playerPosition - Position;
            direction.Normalize();
            Position += direction * speed * elapsed;
        }
    }
}
