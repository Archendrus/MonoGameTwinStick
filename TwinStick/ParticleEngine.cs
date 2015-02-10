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
    // to create particle explosion
    class ParticleEngine
    {
        private Random random;
        public Vector2 EmitterLocation { get; set; }
        private List<Particle> particles;
        private Texture2D texture;
        private float speed = 150f;  // speed at which particles move
        private Vector2 scale;

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
        private Particle GenerateNewParticle(float time, Vector2 min, Vector2 max)
        {
            Vector2 direction = new Vector2(min.X + (float)(random.NextDouble() * (max.X - min.X)),
                                            min.Y + (float)(random.NextDouble() * (max.Y - min.Y)));
            Vector2 velocity = direction * speed;
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
        public void CreateParticles(GameTime gameTime, Vector2 location, int numParticles, float time, Vector2 min, Vector2 max)
        {
            EmitterLocation = location;
            for (int i = 0; i < numParticles; i++)
            {               
                particles.Add(GenerateNewParticle(time, min, max));              
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
    }
}
