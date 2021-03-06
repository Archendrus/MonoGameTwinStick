﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace TwinStick
{
    // EnemyManager handles Zombie creation, update, draw for all Zombies
    // performs collision detection with other Zombies, player, and victim
    // flags HadPlayerCollision and HadVictimCollision are set if collision 
    // of these types occured in the current update loop
    class EnemyManager
    {
        public List<Zombie> Enemies { get; private set; }  // List of enemies
        private float _enemySpawnRate;  // backing field for EnemySpawnRate
        private float _enemySpeed;  // backing field for EnemySpeed
        private Texture2D zombieTexture;  // Texture to use for enemies
        private List<Vector2> spawnPoints;  // list of spawn points
        private float enemySpawnElapsed;  // accumulates time since last enemy spawn        
        private Vector2 scale;
        private ParticleEngine particleEngine;
        private SoundEffect explodeSound;

        // flag if victim collision occured during last update 
        public bool HadVictimCollision { get; private set; }
        public bool HadPlayerCollision { get; private set; }

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

        public EnemyManager(Texture2D zombieTexture, Rectangle screenRect, Vector2 scale, SoundEffect sound, ParticleEngine particleEngine)
        {
            Enemies = new List<Zombie>();
            this.zombieTexture = zombieTexture;
            enemySpawnElapsed = 0;
            EnemySpawnRate = 3.10f;
            EnemySpeed = 20f;
            this.scale = scale;
            explodeSound = sound;
            this.particleEngine = particleEngine;
            InitSpawnPoints(screenRect);
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
                if (victim.IsAlive && Enemies[i].CollisionRect.Intersects(victim.HitBox))
                {
                    HadVictimCollision = true;
                }

                // Check enemy collision with player
                if (player.IsAlive && Enemies[i].HitBox.Intersects(player.HitBox))
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
        private void InitSpawnPoints(Rectangle screenRect)
        {
            spawnPoints = new List<Vector2>();
            float centerY = (screenRect.Height / 2.0f) - (zombieTexture.Height / 2.0f);
            float centerX = (screenRect.Width / 2.0f) - (zombieTexture.Width / 2.0f);
            spawnPoints.Add(new Vector2(0 - (zombieTexture.Width / 2.0f), centerY));
            spawnPoints.Add(new Vector2(screenRect.Width + (zombieTexture.Width / 2.0f), centerY));
            spawnPoints.Add(new Vector2(centerX, 0 - (zombieTexture.Height / 2.0f)));
            spawnPoints.Add(new Vector2(centerX, screenRect.Height + (zombieTexture.Height / 2.0f)));
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
        // and play sound effect, create particles
        public void Kill(int index, GameTime gameTime, Vector2 bulletDirection)
        {
            Enemies[index].IsAlive = false;
            ExplodeEnemy(gameTime, Enemies[index].Center, bulletDirection);
            explodeSound.Play();
        }

        // Create particle explosion at zombie location 
        // using the colliding bullets direction to create a direction range for 
        // each the particles
        private void ExplodeEnemy(GameTime gameTime, Vector2 location, Vector2 bulletDirection)
        {
            int zombieParticles = 30; // number of particles to create 18
            float range = .6f; // range +/- to direction
            float time = .28f; // time before partices stop moving .32f

            // Create min, max vectors
            // based on bullet direcction +/- range
            Vector2 directionMin = new Vector2(bulletDirection.X - range, bulletDirection.Y - range);
            Vector2 directionMax = new Vector2(bulletDirection.X + range, bulletDirection.Y + range);

            // init min, max particle speed
            int speedMin = 150;
            int speedMax = 300;

            particleEngine.CreateParticles(
                gameTime,
                location,
                zombieParticles,
                time,
                directionMin,
                directionMax,
                speedMin,
                speedMax);
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
