using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.Utils
{
    public static class MathS
    {
        private static Matrix2x2 IsoMatrix => new Matrix2x2(new Vector2(1, 0.5f), new Vector2(-1, 0.5f));

        public static Vector2 PositionToIsoPosition(Vector2 pPos)
        {
            return new Vector2(
                pPos.X * IsoMatrix.A.X + pPos.Y * IsoMatrix.B.X,
                pPos.X * IsoMatrix.A.Y + pPos.Y * IsoMatrix.B.Y);
        }

        /// <summary>
        /// </summary>
        /// <param name="pTileSize"></param>
        /// <param name="pIndex"></param>
        /// <returns>A world position according to a grid index and the given tile size</returns>
        public static Vector2 IndexToPosition(Vector2 pTileSize, int pIndexX, int pIndexY)
        {
            Vector2 lScaledSize = pTileSize * 0.5f;
            return new Vector2(
                lScaledSize.X * pIndexX * IsoMatrix.A.X + lScaledSize.Y * pIndexY * IsoMatrix.B.X,
                lScaledSize.X * pIndexX * IsoMatrix.A.Y + lScaledSize.Y * pIndexY * IsoMatrix.B.Y);
        }

        /// <summary>
        /// </summary>
        /// <param name="pTileSize"></param>
        /// <param name="pIndex"></param>
        /// <returns>A world position according to a grid index and the given tile size</returns>
        public static Vector2 IndexToPosition(Vector2 pTileSize, Vector2I pIndex)
        {
            return IndexToPosition(pTileSize, pIndex.X, pIndex.Y);
        }

        /// <summary>
        /// </summary>
        /// <param name="pTileSize"></param>
        /// <param name="pIndex"></param>
        /// <returns>A world position according to a grid index and the given tile size</returns>
        public static Vector2 IndexToPosition(Vector2 pTileSize, Vector2 pIndex)
        {
            return IndexToPosition(pTileSize, (int)pIndex.X, (int)pIndex.Y);
        }

        /// <summary>
        /// Converts from polar coordinates to a vector
        /// </summary>
        /// <param name="pRadius"></param>
        /// <param name="pAngle"></param>
        /// <returns>A Vector2 representing the point on the given circle</returns>
        public static Vector2 PolarToCartesian(float pRadius, float pAngle)
        {
            return pRadius * new Vector2(MathF.Cos(pAngle), MathF.Sin(pAngle));
        }

        /// <summary>
        /// Same as myVector2.IsEqualApprox() but with a tolerence.
        /// </summary>
        /// <param name="pA"></param>
        /// <param name="pB"></param>
        /// <param name="pTolerance"></param>
        /// <returns>True if pA and pB are closer to each other than the tolerance.</returns>
        public static bool ArePosClose(Vector2 pA, Vector2 pB, float pTolerance)
        {
            return Mathf.IsEqualApprox(pA.X, pB.X, pTolerance) && Mathf.IsEqualApprox(pA.Y, pB.Y, pTolerance);
        }

        /// <summary>
        /// </summary>
        /// <param name="pA"></param>
        /// <param name="pB"></param>
        /// <param name="pPoint"></param>
        /// <returns>True if pA is closer to pPoint than pB</returns>
        public static bool IsCloserToPointThan(Vector2 pA, Vector2 pB, Vector2 pPoint)
        {
            return (pA - pPoint).Length() < (pB - pPoint).Length();
        }

        /// <summary>
        /// pEnd must be greater than pStart.
        /// </summary>
        /// <param name="pValue"></param>
        /// <param name="pStart"></param>
        /// <param name="pEnd"></param>
        /// <returns>A percentage representing where pValue sits between pStart and pEnd.</returns>
        public static float PercentageBetweenValues(float pValue, float pStart, float pEnd)
        {
            if (pStart <= pEnd)
                throw new Exception($"{nameof(pEnd)} must be greater than {nameof(pStart)}");
            return (pValue - pStart) / (pStart - pEnd) * 100;
        }

        /// <summary>
        /// <para>Returns a number equally away from pEnd than pValue is from pStart. The distance is flipped.</para>
        /// (Ex : 30, 25, 100, will give off 95 because 30 is 5 away from 25 and 95 is -5 away from 100.)
        /// </summary>
        /// <param name="pValue"></param>
        /// <param name="pStart"></param>
        /// <param name="pEnd"></param>
        /// <returns></returns>
        public static float InverseBetweenValues(float pValue, float pStart, float pEnd)
        {
            return pStart + pEnd - pValue;
        }
    }
}
