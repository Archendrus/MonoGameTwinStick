using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwinStick
{
    class Bullet : Sprite
    {
        private float speed = 300f;
        Vector2 direction;

        public Rectangle CollisionRect
        {
            get
            {
                return BoundingRect;
            }
        }

        public Bullet(Texture2D texture, Vector2 direction) : base(texture)
        {
            this.direction = direction;
        }

        public void Update(GameTime time)
        {
            float elapsed = (float)time.ElapsedGameTime.TotalSeconds;

            Position += direction * speed * elapsed;
        }
    }
}
