using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace TwinStick
{
    // Circle used for collision detection
    // and resolution
    class Circle
    {
        public Vector2 Position { get; private set; }
        public float Radius { get; private set; }

        public Circle(Vector2 position, float radius)
        {
            this.Position = position;
            this.Radius = radius;
        }
        
        // Calulate the intersection depth of this circle with circle
        // Zero returned if circles do not intersect
        // depth of intersection returned if circles do intersect
        public float GetIntersectionDepth(Circle circle)
        {
            float depth = 0;
            float sumOfRadii = this.Radius + circle.Radius;
            float distance = Vector2.Distance(this.Position, circle.Position);
            if (distance < sumOfRadii)
            {
                depth = sumOfRadii - distance;
            }
            return depth;
        }
    }
}
