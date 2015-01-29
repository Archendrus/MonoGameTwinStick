using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStick
{
    class BulletManager
    {
        public List<Bullet> Bullets { get; private set; }
        Texture2D bulletTexture;
        float shotTimerElapsed;

        Vector2 scale;

        public BulletManager(Texture2D bulletTexture, Vector2 scale)
        {
            Bullets = new List<Bullet>();
            this.bulletTexture = bulletTexture;
            this.scale = scale;
            shotTimerElapsed = 0;
        }

        public void Update(GameTime gameTime, Player player, TileMap tileMap, Vector2 shootDirection, Rectangle virtualScreenRect)
        {
            CreateBullets(gameTime, player, shootDirection);

            // Update the bullets
            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Update(gameTime, tileMap, virtualScreenRect);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Draw(spriteBatch);
            }

            CleanupSprites();
        }

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
                    // reset timer
                    shotTimerElapsed = 0;
                }
            }
        }


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

        public void Reset()
        {
            Bullets.Clear();
        }
    }
}
