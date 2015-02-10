using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace TwinStick
{
    // BulletManager handles creation, update, and draw
    // for all bullets in the game
    class BulletManager
    {
        public List<Bullet> Bullets { get; private set; }  // list of bullets
        Texture2D bulletTexture;  // texture to use for bullets
        float shotTimerElapsed;  // accumulates time since last shot
        SoundEffect shotSound;

        Vector2 scale;

        public BulletManager(Texture2D bulletTexture, Vector2 scale, SoundEffect sound)
        {
            Bullets = new List<Bullet>();
            this.bulletTexture = bulletTexture;
            this.scale = scale;
            shotTimerElapsed = 0;
            shotSound = sound;
        }

        public void Update(GameTime gameTime, Player player, TileMap tileMap, Vector2 shootDirection, Rectangle screenRect)
        {
            // Create bullets
            CreateBullets(gameTime, player, shootDirection);

            // Update the bullets
            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Update(gameTime, tileMap, screenRect);
            }

            // Remove inactive bullets from list
            CleanupSprites();
        }

        // draw all bullets in bullets list
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Draw(spriteBatch);
            }     
        }


        // Create bullets at player.Position, moving in shootDirection at fireRate
        public void CreateBullets(GameTime gameTime, Player player, Vector2 shootDirection)
        {
            float fireRate = .30f;
            // accumulate elapsed time
            shotTimerElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            // if input
            if (shootDirection != Vector2.Zero)
            {
                // if fireRate time has passed since last shot
                if (shotTimerElapsed > fireRate)
                {
                    // create a new bullet at player position, in shootDirection
                    // add to bullet list
                    shootDirection.Normalize();
                    Bullet bullet = new Bullet(bulletTexture, shootDirection, scale);
                    bullet.Position = new Vector2(
                        player.Position.X + ((player.Width / 2.0f) - (bullet.Width / 2.0f)),
                        player.Position.Y + ((player.Height / 2.0f) - (bullet.Height / 2.0f)));
                    Bullets.Add(bullet);
                    // reset timer and play sound effect
                    shotTimerElapsed = 0;
                    shotSound.Play();
                }
            }
        }

        // Set bullet at index IsAlive flag to false
        public void Kill(int index)
        {
            Bullets[index].IsAlive = false;
        }

        private void CleanupSprites()
        {
            // Remove inactive bullets
            for (int i = 0; i < Bullets.Count; i++)
            {
                if (!Bullets[i].IsAlive)
                {
                    Bullets.Remove(Bullets[i]);
                }
            }
        }

        // Clear bullet list
        public void Reset()
        {
            Bullets.Clear();
        }
    }
}
