using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.GameObjects.Mobiles
{
    public partial class Sheep : Mobile
    {
        public Vector2I Direction { get; private set; }
        public bool IsUseful;

        public bool CanMove()
        {
            //Add movement test logic here. Used in other classes.
            return true;
        }

        public static Sheep Create(PackedScene pScene, Tile pTile, Vector2I pDirection, bool pUseful = true)
        {
            Sheep lSheep = Create<Sheep>(pScene, pTile);
            lSheep.Direction = pDirection.Sign();
            lSheep.IsUseful = pUseful;
            return lSheep;
        }

        public static Sheep Create(PackedScene pScene, Vector2I pTileIndex, Vector2I pDirection, bool pUseful = true)
        {
            return Create(pScene, GridManager.TileDict[pTileIndex], pDirection, pUseful);
        }
    }
}
