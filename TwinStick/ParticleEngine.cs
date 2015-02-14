using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStick
{
    // ParticleEngine creates, updates and draws particles
    // Call CreateParticles(gameTime, location, numParticles, time, min, max)
    // to create particles with custom values
    // or call ExplodeSprite(gameTime, location) for generic explosion at location
    class ParticleEngine
    {
        private Random random;
        public Vector2 EmitterLocation { get; set; }
        public int Bursts { get; set; }
        private List<Particle> particles;
        private Texture2D texture;
        private Vector2 scale;

        // Create a particle engine using texture, set EmitterLocation to location
        public ParticleEngine(Texture2D texture, Vector2 location, Vector2 scale)
        {
            EmitterLocation = location;
            this.texture = texture;
            this.particles = new List<Particle>();
            this.scale = scale;
            random = new Random();
        }

        // Update all particles
        public void Update(GameTime gameTime, TileMap map)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update(gameTime, map);
            }

            // Remove dead particles
            CleanupParticles();
        }

        // Generate a single particle with a random direction vector between
        // min and max vectors
        // and add it to particles list
        private Particle GenerateNewParticle(float time, Vector2 directionMin, Vector2 directionMax, int speedMin, int speedMax)
        {
            // Choose random direction vector between min and max
            Vector2 direction = new Vector2(directionMin.X + (float)(random.NextDouble() * (directionMax.X - directionMin.X)),
                                            directionMin.Y + (float)(random.NextDouble() * (directionMax.Y - directionMin.Y)));

            // choose random speed between speedMin, speedMax
            float particleSpeed = (float)random.Next(speedMin, speedMax);

            // set velocity and return new particle
            direction.Normalize();
            Vector2 velocity = direction * particleSpeed;
            return new Particle(texture,EmitterLocation, velocity, time, scale);
        }


        // Draw all particles to the screen using alpha value
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                Particle particle = particles[i];
                particle.Draw(spriteBatch, particle.Alpha);
            }
        }

        // create numParticles at ParticleEmitters current location
        public void CreateParticles(
            GameTime gameTime,
            Vector2 location,
            int numParticles,
            float time,
            Vector2 directionMin,
            Vector2 directionMax,
            int speedMin,
            int speedMax)
        {
            EmitterLocation = location;
            for (int i = 0; i < numParticles; i++)
            {               
                particles.Add(GenerateNewParticle(time, directionMin, directionMax, speedMin, speedMax));              
            }    
        }

        // Remove dead particles from list
        private void CleanupParticles()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                if (!particles[i].IsAlive)
                {
                    particles.Remove(particles[i]);
                }
            }
        }

        // Reset the particle engine
        public void Reset()
        {
            particles.Clear();
        }

        // Set values and create particles for victim and player death
        // explosions
        public void ExplodeSprite(GameTime gameTime, Vector2 location)
        {
            int particles = 80; // number of particles to create 18
            float time = .20f; // time before partices stop moving .32f

            // Create min, max vectors
            // based on bullet direcction +/- range
            Vector2 directionMin = new Vector2(-1, -1);
            Vector2 directionMax = new Vector2(1, 1);

            // init min, max particle speed
            int speedMin = 50;
            int speedMax = 300;

            CreateParticles(
                gameTime,
                location,
                particles,
                time,
                directionMin,
                directionMax,
                speedMin,
                speedMax);
        }
    }
}
