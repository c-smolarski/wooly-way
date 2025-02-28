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
        
        public enum Step
        {
            First,
            Second,
        }
        
        /// <summary>
        /// Checks if the current Sheep can move onto the next Tile depending on if it's its first or second step and the content of the lNextTile.
        /// </summary>
        /// <param name="pMoveDirection"></param>
        /// <param name="pStep"></param>
        /// <returns>true if the Sheep can be moved onto the next tile. Otherwise, false.</returns>
        public bool CanMove(Vector2I pMoveDirection, int pStep)
        {
            Tile lNextTile = Grid.IndexDict[Grid.IndexDict[Grid.ObjectDict[this]] + pMoveDirection];

            if (lNextTile.IsEmpty()) return true;

            switch (pStep)
            {
                case (int)Step.First:
                    return false;
                case (int)Step.Second:
                    return Grid.ObjectDict[lNextTile] is Sheep && (Grid.ObjectDict[lNextTile] as Sheep).CanMove(pMoveDirection, (int)Step.First);

            }
            
            return lNextTile.IsWalkable(pMoveDirection);
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
