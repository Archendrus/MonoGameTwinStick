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

        // Circle for collision centered on sprite
        public Circle CollisionCircle
        {
            get
            {
                return new Circle(new Vector2(Center.X, Center.Y), 8.0f * scale.X);
            }
        }

        public Zombie(Texture2D texture, Vector2 position, float speed, Vector2 scale)
            : base(texture, position, scale)
        {
            this.speed = speed;
        }

        public Zombie(Texture2D texture, float speed, Vector2 scale)
            : base(texture, scale)
        {
            this.speed = speed;
        }

        public void Update(GameTime time, TileMap map, Player player, Victim victim)
        {
            if (IsAlive)
            {
                float elapsed = (float)time.ElapsedGameTime.TotalSeconds;
                Vector2 direction = Vector2.Zero;

                // if there is a victim, decide which direction to move
                if (victim.IsAlive)
                {
                    // if victim is closer move towards victim
                    if (Vector2.Distance(Center, victim.Center) < Vector2.Distance(Center, player.Center))
                    {
                        direction = victim.Position - Position;
                    }
                    else // move towards player
                    {
                        direction = player.Position - Position;
                    }
                }
                else // no victim, move towards player
                {
                    direction = player.Position - Position;
                }

                // Update position and check tile collisions
                direction.Normalize();
                Position += direction * speed * elapsed;
                ResolveTileCollisions(map);
            }
            
        }

        // Resolve tile collsions on both axis at once
        // using the smallest axis to determine collision side
        private void ResolveTileCollisions(TileMap map)
        {
            List<Tile> collisionTiles = map.CheckTileCollsions(CollisionRect);
            foreach (Tile tile in collisionTiles)
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
