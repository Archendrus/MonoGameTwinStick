using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStick
{
    // Player bullet sprite. travels in direction set in constructor
    // at speed.  Performs tile collision with map
    class Bullet : Sprite
    {
        private float speed = 500f;
        public Vector2 Direction { get; private set; }
        private List<Tile> collisionTiles = new List<Tile>();

        // Collision rect same as bounding rect for bullet
        public Rectangle CollisionRect
        {
            get
            {
                return BoundingRect;
            }
        }

        // Create bullet moving in direction
        public Bullet(Texture2D texture, Vector2 direction, Vector2 scale) : base(texture, scale)
        {
            Direction = direction;
        }

        public void Update(GameTime time, TileMap map, Rectangle screenRect)
        {
            // Only update if Bullet.IsAlive
            if (IsAlive)
            {
                // update position every frame
                float elapsed = (float)time.ElapsedGameTime.TotalSeconds;

                Position += Direction * speed * elapsed;

                // kill if offscreen
                if (!screenRect.Contains(BoundingRect))
                {
                    IsAlive = false;
                }

                // Check for collision with tiles
                collisionTiles = map.CheckTileCollsions(CollisionRect);
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
}
