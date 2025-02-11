using System;
using System.Reflection;
using Godot;

namespace Com.IsartDigital.WoolyWay.Utils
{
    public static class MathS
    {
        public static Matrix2x2 IsoMatrix => new Matrix2x2(new Vector2(1, 0.5f), new Vector2(-1, 0.5f));

        public static Vector2 IndexToPosition(Vector2 pIndex)
        {
            Vector2 lScaledSize = GridManager.Instance.TileSize * GridManager.Instance.TileScale * 0.5f;
            return new Vector2(
                        lScaledSize.X * pIndex.X * IsoMatrix.A.X + lScaledSize.Y * pIndex.Y * IsoMatrix.B.X,
                        lScaledSize.X * pIndex.X * IsoMatrix.A.Y + lScaledSize.Y * pIndex.Y * IsoMatrix.B.Y);
        }

        public static Vector2 IndexToPosition(float pIndexX, float pIndexY)
        {
            return IndexToPosition(new Vector2(pIndexX, pIndexY));
        }
    }
}
