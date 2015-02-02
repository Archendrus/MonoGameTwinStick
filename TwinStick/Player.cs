using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TwinStick
{
    class Player : Sprite
    {
        
        public Vector2 Direction { get; set;}
        private float speed;
        
        // Rectangle for collison with tiles
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

       // Rectangle for collision with enemies
       public Rectangle HitBox
        {
            get
            {
                return new Rectangle(
                    (int)Position.X + (5 * (int)scale.X),
                    (int)Position.Y + (1 * (int)scale.Y),
                    (5 * (int)scale.X),
                    (14 * (int)scale.Y));
            }
        }
        
        // 6px in, 4 width, 3 down, 10 height
        public Player(Texture2D texture, Vector2 position, Vector2 scale) 
            : base(texture, position, scale)
        {

            Direction = new Vector2(0, 0);
            speed = 175f;
        }

        public Player(Texture2D texture, Vector2 scale)
            : base(texture, scale)
        {
            Direction = new Vector2(0, 0);
            speed = 175f;
        }

        public void Update(GameTime time, TileMap map, Vector2 direction, Rectangle virtualScreenRect)
        {
            float elapsed = (float)time.ElapsedGameTime.TotalSeconds;
            
            // If input, move and check/resolve collisions
            if (direction != Vector2.Zero)
            {
                // normalize vector for diagonal movement
                direction.Normalize();

                // move horizontal, check and resolve collisions
                Position += new Vector2(direction.X * speed * elapsed, 0);
                Vector2 axis = new Vector2(direction.X, 0);
                ResolveTileCollisions(map, axis);

                // move vertical, check and resolve collisions
                Position += new Vector2(0, direction.Y * speed * elapsed);
                axis = new Vector2(0, direction.Y);
                ResolveTileCollisions(map, axis);
            }

            // Wrap around the screen in all directions
            if (Position.X > virtualScreenRect.Width)
            {
                Position = new Vector2(-BoundingRect.Width, Position.Y);
            }
            if (Position.X < -BoundingRect.Width)
            {
                Position = new Vector2(virtualScreenRect.Width, Position.Y);
            }
            if (Position.Y > virtualScreenRect.Height)
            {
                Position = new Vector2(Position.X, -BoundingRect.Height);
            }
            if (Position.Y < -BoundingRect.Height)
            {
                Position = new Vector2(Position.X, virtualScreenRect.Height);
            }
        }

        // Resolve tile collisions for axis passed in
        private void ResolveTileCollisions(TileMap map, Vector2 axis)
        {
            List<Tile> collisionTiles = map.CheckTileCollsions(CollisionRect);
            foreach (Tile tile in collisionTiles)
            {
                // Resolve solid tile collision
                if (tile.IsSolid)
                {
                    Vector2 depth;

                    // Resolve collisions on the X axis
                    if (axis.X != 0)
                    {
                        depth = new Vector2(RectangleExtensions.GetHorizontalIntersectionDepth(CollisionRect, tile.BoundingRect), 0);
                    }
                    else // Resolve collisions on the Y axis
                    {
                        depth = new Vector2(0, RectangleExtensions.GetVerticalIntersectionDepth(CollisionRect, tile.BoundingRect));
                    }

                    // move player out of tile by depth
                    Position += depth;
                }
            }
        }
    }
}
