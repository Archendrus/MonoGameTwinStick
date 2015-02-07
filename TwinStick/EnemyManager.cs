using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TwinStick
{
    class EnemyManager
    {
        public List<Zombie> Enemies { get; private set; }  // List of enemies
        private float _enemySpawnRate;  // backing field for EnemySpawnRate
        private float _enemySpeed;  // backing field for EnemySpeed

        // Rate at which enemies spawn
        public float EnemySpawnRate 
        {
            get
            {
                return _enemySpawnRate;
            }
            
            set
            {
                // clamp enemy spawn rate at 1.20f
                _enemySpawnRate = value;
                if (_enemySpawnRate < 1.20f)
                {
                    _enemySpawnRate = 1.20f;
                }
            } 
        }


        // Enemy movement speed
        public float EnemySpeed 
        { 
            get
            {
                return _enemySpeed;
            }
 
            set
            {
                // clamp movement speed at 32f
                _enemySpeed = value;
                if (_enemySpeed > 32f)
                {
                    _enemySpeed = 32f;
                }
            }
        }

        private Texture2D zombieTexture;  // Texture to use for enemies
        private List<Vector2> spawnPoints;  // list of spawn points
        private float enemySpawnElapsed;  // accumulates time since last enemy spawn
        
        private Vector2 scale;

        // flag if victim collision occured during last update 
        public bool HadVictimCollision { get; private set; }
        public bool HadPlayerCollision { get; private set; }

        public EnemyManager(Texture2D zombieTexture, Rectangle virtualScreenRect, Vector2 scale)
        {
            Enemies = new List<Zombie>();
            this.zombieTexture = zombieTexture;
            enemySpawnElapsed = 0;
            EnemySpawnRate = 3.10f;
            EnemySpeed = 20f;
            this.scale = scale;

            InitSpawnPoints(virtualScreenRect);
        }

        // Update all enemies in enemy list
        public void Update(GameTime gameTime, Player player, TileMap tileMap, Victim victim)
        {
            // Spawn enemies
            SpawnEnemies(gameTime);

            // Reset victim collision flag
            HadVictimCollision = false;
            HadPlayerCollision = false;

            // Update all enemies
            // if enemy collides with victim, set victim collision flag
            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemies[i].Update(gameTime, tileMap, player, victim);

                // Check enemy collision with victim
                if (victim.IsAlive && Enemies[i].CollisionRect.Intersects(victim.CollisionRect))
                {
                    HadVictimCollision = true;
                }

                // Check enemy collision with player
                if (Enemies[i].HitBox.Intersects(player.HitBox))
                {
                    HadPlayerCollision = true;
                }
            }

            // Check and resolve collision between enemies
            ResolveEnemyCollision();

            //  Remove inacive enemies from list
            CleanupSprites();
        }

        // Draw all enemies in enemy list
        public void Draw(SpriteBatch spriteBatch)
        {
            // draw all enemies over player
            for (int i = 0; i < Enemies.Count; i++)
            {
                Enemies[i].Draw(spriteBatch);
            }
        }


        // Create a list of four enemy spawn points at N,S,E, and W
        private void InitSpawnPoints(Rectangle virtualScreenRect)
        {
            spawnPoints = new List<Vector2>();
            float centerY = (virtualScreenRect.Height / 2.0f) - (zombieTexture.Height / 2.0f);
            float centerX = (virtualScreenRect.Width / 2.0f) - (zombieTexture.Width / 2.0f);
            spawnPoints.Add(new Vector2(0 - (zombieTexture.Width / 2.0f), centerY));
            spawnPoints.Add(new Vector2(virtualScreenRect.Width + (zombieTexture.Width / 2.0f), centerY));
            spawnPoints.Add(new Vector2(centerX, 0 - (zombieTexture.Height / 2.0f)));
            spawnPoints.Add(new Vector2(centerX, virtualScreenRect.Height + (zombieTexture.Height / 2.0f)));
        }

        // spawn an enemy at each point in spawnPoints at spawnRate
        private void SpawnEnemies(GameTime gameTime)
        {
            enemySpawnElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (enemySpawnElapsed > EnemySpawnRate)
            {
                foreach (Vector2 spawnPoint in spawnPoints)
                {
                    Enemies.Add(new Zombie(zombieTexture, spawnPoint, EnemySpeed, scale));
                }
                // reset timer
                enemySpawnElapsed = 0;
            }
        }

        // Check for and resolve collisions between enemies
        // if two enemies collide, push them back in opposite directions
        private void ResolveEnemyCollision()
        {
            for (int i = 0; i < Enemies.Count; i++)
            {
                for (int j = i + 1; j < Enemies.Count; j++)
                {
                    Zombie zombie1 = Enemies[i] as Zombie;
                    Zombie zombie2 = Enemies[j] as Zombie;

                    // Check if enemies are colliding and get the depth of collision
                    float depth = zombie1.CollisionCircle.GetIntersectionDepth(zombie2.CollisionCircle);

                    // Collision
                    if (depth != 0)
                    {
                        // Get direction to move first zombie away from second zombie
                        Vector2 direction = zombie1.CollisionCircle.Position - zombie2.CollisionCircle.Position;
                        direction.Normalize();

                        // Move first zombie away half the depth of collision
                        zombie1.Position += direction * (depth / 2.0f);
                        // Move second zombie in the opposite direction half the depth of collision
                        zombie2.Position -= direction * (depth / 2.0f);
                    }
                }
            }
        }

        // Set enemy at index IsAlive flag to false
        public void Kill(int index)
        {
            Enemies[index].IsAlive = false;
        }

        // Remove all inactive sprites from bullets and enemies
        private void CleanupSprites()
        {
            // Remove inactive enemies
            for (int i = 0; i < Enemies.Count; i++)
            {
                if (!Enemies[i].IsAlive)
                {
                    Enemies.Remove(Enemies[i]);
                }
            }
        }


        // Clear enemy list
        public void Reset()
        {
            Enemies.Clear();
            enemySpawnElapsed = 0;
        }
    }
}
