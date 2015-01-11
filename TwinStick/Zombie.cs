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
                return new Rectangle(
                    (int)Position.X + (4 * (int)Game1.Scale.X),
                    (int)Position.Y,
                    (8 * (int)Game1.Scale.X),
                    Height);
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
            ResolveTileCollisions(map);
        }

        private void ResolveTileCollisions(TileMap map)
        {
            List<Tile> tiles = map.CheckTileCollsions(CollisionRect);
            foreach (Tile tile in tiles)
            {
                // Resolve solid tile collision
                if (tile.IsSolid)
                {
                    // Determine collision depth (with direction) and magnitude.
                    Vector2 depth = RectangleExtensions.GetIntersectionDepth(CollisionRect, tile.BoundingRect);
                    if (depth != Vector2.Zero)
                    {
                        float absDepthX = Math.Abs(depth.X);
                        float absDepthY = Math.Abs(depth.Y);

                        // Find the smallest axis, this is the side in which collision occurred.
                        // Check if Y is the smallest axis
                        if (absDepthY < absDepthX)
                        {
                            // Resolve the collision along the Y axis.
                            Position = new Vector2(Position.X, Position.Y + depth.Y);
                        }
                        else // X is the smallest axis
                        {
                            // Resolve the collision along the X axis.
                            Position = new Vector2(Position.X + depth.X, Position.Y);
                        }
                    }
                }
            } 
        }
    }
}
