using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStick
{
    class Particle : Sprite
    {
        Vector2 velocity;
        float angle;
        float angularVelocity;
        Vector2 origin;

        float created;
        public float Lifetime { get; private set; }

        float elapsedTime;
         
        public Particle(Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, float created, float lifetime, Vector2 scale)
            : base(texture, position, scale)
        {
            this.velocity = velocity;
            this.angle = angle;
            this.angularVelocity = angularVelocity;
            this.created = created;
            this.Lifetime = lifetime;
            elapsedTime = 0;
            origin = new Vector2((this.Width / scale.X) / 2f, (this.Height / scale.Y) / 2f);
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsedTime += elapsed;
            if (elapsedTime - created > Lifetime)
            {
                IsAlive = false;
            }
            else
            {
                Position += velocity * elapsed;
                angle += angularVelocity * elapsed;
            }
        }
    }
}
