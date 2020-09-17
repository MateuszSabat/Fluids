using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fluids._2D {
    [System.Serializable]
    public struct Material
    {
        public Color color;
        public float radious;
        public float solidRadious;
        public float maxRadious;

        public static int stride
        {
            get { return 7 * sizeof(float); }
        }
    }
}
