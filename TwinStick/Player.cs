﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace TwinStick
{
    // Player sprite
    // Handles movement from input and performs tile collision with map
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
                    (int)Position.X + (4 * (int)scale.X),
                    (int)Position.Y + (3 * (int)scale.Y),
                    (8 * (int)scale.X),
                    (10 * (int)scale.Y));
            }
        }

        SoundEffect dieSound;
        SoundEffectInstance soundEffectInstance;
        ParticleEngine particleEngine;
        
        public Player(Texture2D texture, Vector2 position, Vector2 scale, SoundEffect dieSound, ParticleEngine particleEngine) 
            : base(texture, position, scale)
        {

            Direction = new Vector2(0, 0);
            speed = 175f;
            this.dieSound = dieSound;
            this.particleEngine = particleEngine;
        }

        public Player(Texture2D texture, Vector2 scale, SoundEffect dieSound, ParticleEngine particleEngine)
            : base(texture, scale)
        {
            Direction = new Vector2(0, 0);
            speed = 175f;
            this.dieSound = dieSound;
            this.particleEngine = particleEngine;
        }

        public void Update(GameTime time, TileMap map, Vector2 direction, Rectangle screenRect)
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
            if (Position.X > screenRect.Width)
            {
                Position = new Vector2(-BoundingRect.Width, Position.Y);
            }
            if (Position.X < -BoundingRect.Width)
            {
                Position = new Vector2(screenRect.Width, Position.Y);
            }
            if (Position.Y > screenRect.Height)
            {
                Position = new Vector2(Position.X, -BoundingRect.Height);
            }
            if (Position.Y < -BoundingRect.Height)
            {
                Position = new Vector2(Position.X, screenRect.Height);
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

        // Kill player
        // Set IsAlive to false
        // play die sound as SoundEffectInstance, create particles
        public void Kill(GameTime gameTime)
        {
            IsAlive = false;  
            soundEffectInstance = dieSound.CreateInstance();
            soundEffectInstance.Play();
            particleEngine.ExplodeSprite(gameTime, Center);
             
        }

        // Returns true if dieSound SoundEffectInstance 
        // is not null and done playing
        public bool Dead()
        {
            bool dead = false;
            
            if (soundEffectInstance != null)
            {
                // If sound done playing set dead to true and null out
                // soundEffectInstance
                if (soundEffectInstance.State == SoundState.Stopped)
                {
                    dead = true;
                    soundEffectInstance = null;
                }
            }

            return dead;
        }
    }
}
