using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fluids._2D
{
    public struct Particle
    {
        public Vector2 pos;
        public int material;

        public static int stride
        {
            get { return 2 * sizeof(float) + sizeof(int); }
        }

        public Particle(Vector2 _pos, int _material = 0)
        {
            pos = _pos;
            material = _material;
        }
        public Particle(float _x, float _y, int _material = 0)
        {
            pos = new Vector2(_x, _y);
            material = _material;
        }
    }
}
