using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStick
{
    class Bullet : Sprite
    {
        private float speed = 500f;
        private Vector2 direction;
        private List<Tile> collisionTiles = new List<Tile>();

        public bool IsActive { get; private set; }

        // Collision rect same as bounding rect for bullet
        public Rectangle CollisionRect
        {
            get
            {
                return BoundingRect;
            }
        }

        public Bullet(Texture2D texture, Vector2 direction) : base(texture)
        {
            this.direction = direction;
            IsActive = true;
        }

        public void Update(GameTime time, TileMap map)
        {
            // update position every frame
            float elapsed = (float)time.ElapsedGameTime.TotalSeconds;

            Position += direction * speed * elapsed;

            // Check for collision with tiles
            collisionTiles = map.CheckTileCollsions(CollisionRect);
            foreach (Tile tile in collisionTiles)
            {
                if (tile.IsSolid)
                {
                    IsActive = false;
                }
            }
        }
    }
}
