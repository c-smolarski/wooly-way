using Com.IsartDigital.WoolyWay.Components;
using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;
using System.Collections.Generic;

// Author : Camille SMOLARSKI & Tom BAGNARA & Alissa DELATTRE

namespace Com.IsartDigital.WoolyWay.GameObjects.Mobiles
{
    public partial class Sheep : Mobile
    {
        public Vector2I Direction { get; private set; }
        public bool IsUseful;
        [Export] private ClickableArea clickable;

        public override void _Ready()
        {
            base._Ready();
            clickable.Clicked += Clicked;
        }

        /// <summary>
        /// Checks a player is next the sheep that was clicked, if yes the sheep moves and the player takes it place
        /// </summary>
        private void Clicked()
        {
            List<Tile> lNeighbors = Grid.Neighbors(CurrentTile);
            int lNumNeighbors = lNeighbors.Count;
            for (int i = 0; i < lNumNeighbors; i++)
            {
                if (lNeighbors.Contains(Grid.ObjectDict[Player.GetInstance()]))
                {
                    Vector2I lDirection =  Grid.IndexDict[CurrentTile] - Grid.IndexDict[Grid.ObjectDict[Player.GetInstance()]];
                    Player.GetInstance().InitMove(lDirection);
                    return;
                }
            }
        }

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
