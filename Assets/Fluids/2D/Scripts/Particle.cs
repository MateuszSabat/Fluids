using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fluids._2D
{
    public struct Particle
    {
        public  Vector2 pos;

        public static int stride
        {
            get { return 2 * sizeof(float); }
        }

        public Particle(Vector2 _pos)
        {
            pos = _pos;
        }
        public Particle(float _x, float _y)
        {
            pos = new Vector2(_x, _y);
        }
    }
}
