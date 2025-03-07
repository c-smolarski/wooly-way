using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

// Author : Camille SMOLARSKI & Tom BAGNARA

namespace Com.IsartDigital.WoolyWay.GameObjects.Mobiles
{
    public partial class Sheep : Mobile
    {
        public Vector2I Direction { get; private set; }
        public bool IsUseful;

        
        
        
        /// <summary>
        /// Checks if the current Sheep can move onto the next Tile depending on if it's its first or second step and the content of the lNextTile.
        /// </summary>
        /// <param name="pMoveDirection"></param>
        /// <returns>true if the Sheep can be moved onto the next tile. Otherwise, false.</returns>
        public bool CanMove(Vector2I pMoveDirection)
        {
            Tile lNextTile = Grid.IndexDict[Grid.IndexDict[CurrentTile] + pMoveDirection];

            if (lNextTile.IsEmpty()) return true;
            
            if (Grid.ObjectDict[lNextTile] is Obstacle || Grid.ObjectDict[lNextTile] is Player) return false;
            
            if (Grid.ObjectDict[lNextTile] is not Sheep) return true;

            return false;
        }


        public override void InitMove(Vector2I pMoveDirection)
        {
            Tile lNextTile = Grid.IndexDict[Grid.IndexDict[CurrentTile] + pMoveDirection];
            Move(lNextTile);
            base.InitMove(Direction);
        }


        public static Sheep Create(PackedScene pScene, Tile pTile, Vector2I pDirection, bool pUseful = true)
        {
            Sheep lSheep = Create<Sheep>(pScene, pTile);
            lSheep.Direction = pDirection.Sign();
            lSheep.IsUseful = pUseful;
            return lSheep;
        }
    }
}
