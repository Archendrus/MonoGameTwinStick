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
        private GamePadState gamePad;
        private Vector2 direction;
        private float speed;
        private KeyboardState key;
        
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

        public Player(Texture2D texture, Vector2 position) 
            : base(texture, position)
        {

            direction = new Vector2(0, 0);
            speed = 175f;
        }

        public Player(Texture2D texture)
            : base(texture)
        {
            direction = new Vector2(0, 0);
            speed = 175f;
        }

        public void Update(GameTime time, TileMap map)
        {
            float elapsed = (float)time.ElapsedGameTime.TotalSeconds;
            
            // Stop moving
            direction = Vector2.Zero;

            // Check gampad
            // Set direction vector values
            gamePad = GamePad.GetState(PlayerIndex.One);
            if (gamePad.IsConnected)
            {
                // Move left
                if (gamePad.ThumbSticks.Left.X < 0)
                {
                    direction.X = -1f;
                }
                // Move right
                else if (gamePad.ThumbSticks.Left.X > 0)
                {
                    direction.X = 1f;
                }
                // Move up
                if (gamePad.ThumbSticks.Left.Y > 0)
                {
                    direction.Y = -1f;
                }
                // Move down
                else if (gamePad.ThumbSticks.Left.Y < 0)
                {
                    direction.Y = 1f;
                }
            }

            // Check keyboard
            // Set direction vector values
            key = Keyboard.GetState();
            // move left
            if (key.IsKeyDown(Keys.A))
            {
                direction.X = -1f;
            }
            // move right
            if (key.IsKeyDown(Keys.D))
            {
                direction.X = 1f;
            }
            // move up
            if (key.IsKeyDown(Keys.W))
            {
                direction.Y = -1f;
            }
            // move down
            if (key.IsKeyDown(Keys.S))
            {
                direction.Y = 1f;
            }

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
        }

        private void ResolveTileCollisions(TileMap map, Vector2 axis)
        {
            List<Tile> tiles = map.CheckTileCollsions(CollisionRect);
            foreach (Tile tile in tiles)
            {
                // Resolve solid tile collision
                if (tile.IsSolid)
                {
                    Vector2 depth;
                    if (axis.X != 0)
                    {
                        depth = new Vector2(RectangleExtensions.GetHorizontalIntersectionDepth(CollisionRect, tile.BoundingRect), 0);
                    }
                    else
                    {
                        depth = new Vector2(0, RectangleExtensions.GetVerticalIntersectionDepth(CollisionRect, tile.BoundingRect));
                    }

                    Position += depth;
                }
            }
        }
    }
}
