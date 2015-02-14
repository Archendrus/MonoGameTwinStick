using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TwinStick
{
    // Particle sprite
    // A single particle moves at velocity for TimeToMove seconds
    // in direction, waits for timeToFade seconds before fading out
    // at fadeRate
    class Particle : Sprite
    {
        public Vector2 Velocity { get; set; }
        public float TimeToMove { get; set; }  // Time before particle stops moving
        public float Alpha { get; private set; } 
        private float timeToFade = 2.0f;  // Time before particle begins fading
        private float fadeRate = .02f;
        //private List<Tile> collisionTiles = new List<Tile>();

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity, float timeToMove, Vector2 scale)
            : base(texture, position, scale)
        {
            Velocity = velocity;
            TimeToMove = timeToMove;
            Alpha = 1.0f;
        }

        public void Update(GameTime gameTime, TileMap map)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move particle at velocity during TimeToMove duration
            TimeToMove -= elapsed;
            if (TimeToMove > 0.0f)
            {
                Position += Velocity * elapsed;
                // Resolve particle collision with nearby tiles
                ResolveTileCollisions(map);
            }
            else  // Particle landed
            {
                // decrement fade time
                timeToFade -= elapsed;

                // If fade timer complete, start fading out
                if (timeToFade < 0.0f)
                {
                    Alpha -= fadeRate;  
                } 
            } 
            
            // If faded all the way out, set IsAlive flag 
            // to false for cleanup
            if (Alpha < 0.0f)
            {
                IsAlive = false;
            }         
        }

        // Resolve tile collsions on both axis at once
        // using the smallest axis to determine collision side
        private void ResolveTileCollisions(TileMap map)
        {
            List<Tile> collisionTiles = map.CheckTileCollsions(BoundingRect);
            foreach (Tile tile in collisionTiles)
            {
                // Resolve solid tile collision
                if (tile.IsSolid)
                {
                    // Determine collision depth (with direction) and magnitude.
                    Vector2 depth = RectangleExtensions.GetIntersectionDepth(BoundingRect, tile.BoundingRect);
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

            // If any particles pass through solid tiles, remove them
            collisionTiles = map.CheckTileCollsions(BoundingRect);
            foreach (Tile tile in collisionTiles)
            {
                if (tile.IsSolid)
                {
                    IsAlive = false;
                }
            }
        }
    }
}
