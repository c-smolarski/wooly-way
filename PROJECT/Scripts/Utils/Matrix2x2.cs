using System;
using Godot;

namespace Com.IsartDigital.WoolyWay.Utils
{
    public struct Matrix2x2
    {
        public Vector2 A { get; private set; }
        public Vector2 B { get; private set; }

        public Matrix2x2(Vector2 pA, Vector2 pB)
        {
            A = pA;
            B = pB;
        }

        public Matrix2x2(float pAx, float pAy, float pBx, float pBy)
        {
            A = new Vector2(pAx, pAy);
            B = new Vector2(pBx, pBy);
        }
    }
}
