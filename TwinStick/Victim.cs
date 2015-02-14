using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace TwinStick
{
    // Victim sprite
    class Victim : Sprite
    {
        private SoundEffect dieSound;
        private SoundEffectInstance soundEffectInstance;
        private ParticleEngine particleEngine;

        // Rectangle for collision based on non-transparent area of sprite
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

        // Create victim with texture, at position, at using scale
        public Victim(Texture2D texture, Vector2 position, SoundEffect dieSound, Vector2 scale, ParticleEngine particleEngine)
            : base(texture, position, scale)
        {
            this.dieSound = dieSound;
            this.particleEngine = particleEngine;
        }

        // Create victim with texutre, using scale
        // (Set position after creation)
        public Victim(Texture2D texture, SoundEffect dieSound, Vector2 scale, ParticleEngine particleEngine)
            : base(texture, scale)
        {
            this.dieSound = dieSound;
            this.particleEngine = particleEngine;
        }

        // Kill victim
        // play sound, create particles, set IsAlive to false
        public void Kill(GameTime gameTime)
        {
            soundEffectInstance = dieSound.CreateInstance();
            soundEffectInstance.Play();
            particleEngine.ExplodeSprite(gameTime, Center);
            IsAlive = false;           
        }

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
