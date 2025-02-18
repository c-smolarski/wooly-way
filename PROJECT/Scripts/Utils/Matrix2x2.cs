using Godot;

// Author : Camille Smolarski

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

        public static Matrix2x2 operator +(Matrix2x2 pA, Matrix2x2 pB)
        {
            return new Matrix2x2(pA.A + pB.A, pA.B + pB.B);
        }

        public static Matrix2x2 operator -(Matrix2x2 pA, Matrix2x2 pB)
        {
            return new Matrix2x2(pA.A - pB.A, pA.B - pB.B);
        }

        public static Matrix2x2 operator *(Matrix2x2 pA, Matrix2x2 pB)
        {
            float lAX = pA.A.X * pB.A.X + pA.A.Y * pB.B.X;
            float lAY = pA.A.X * pB.A.Y + pA.A.Y * pB.B.Y;
            float lBX = pA.B.X * pB.A.X + pA.B.Y * pB.B.X;
            float lBY = pA.B.X * pB.A.Y + pB.B.Y * pB.B.Y;
            return new Matrix2x2(lAX, lAY, lBX, lBY);
        }
    }
}
