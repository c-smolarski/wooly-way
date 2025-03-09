using Com.IsartDigital.ProjectName;
using Com.IsartDigital.WoolyWay.Components;
using Com.IsartDigital.WoolyWay.GameObjects;
using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// Author : Camille SMOLARSKI & Tom BAGNARA && Alissa Delattre

namespace Com.IsartDigital.WoolyWay
{
    public partial class Tile : Node2D
    {
        public static readonly Vector2 SIZE = new(88, 88);

        [Export] private ClickableArea clickable;
        [Export] public Label debug;

        private Pathfinding pathfinding = new Pathfinding();
        public Grid Grid { get; private set; }
        public GameObject CurrentObject => Grid.ObjectDict[this];
        public override void _Ready()
        {
            base._Ready();
            clickable.Clicked += SendCoord;
        }
        /// <summary>
        /// Checks if an object can move on Tile. If the Tile has a Sheep on it, checks if the Sheep can move out of the Tile.
        /// </summary>
        /// <param name="pMoveDirection"></param>
        /// <returns>Bool</returns>
        public bool IsWalkable(Vector2I pMoveDirection)
        {
            if (this.IsEmpty()) return true;
            if (Grid.ObjectDict[this] is Obstacle || Grid.ObjectDict[this] is Player) return false;
            if (!this.IsSheep()) return true;

            Sheep lSheep = Grid.ObjectDict[this] as Sheep;

            return lSheep != null && lSheep.CanMove(pMoveDirection);
        }
        
        public bool IsEmpty()
        {
            return !Grid.ObjectDict.Contains(this);
        }

		public bool IsSheep()
        {
            if (!IsEmpty()) return (Grid.ObjectDict[this] is Sheep);
            return false;
        }

        /// <summary>
        /// Gets the position of the tile that was clicked on and send it to the pathfinding as the target then calls the function to move the player
        /// </summary>
        private void SendCoord()
        {
            Player lPlayer = Player.GetInstance();
            Vector2I lTargetPos = Grid.IndexDict[this];
            Vector2I LStartPos = Grid.IndexDict[Grid.ObjectDict[lPlayer]];
            List<Vector2I> lPath = pathfinding.GetPath(LStartPos.X, LStartPos.Y, lTargetPos.X, lTargetPos.Y);
            if (lPath.Count == 0) return; 
            lPath.Remove(lPath.Last());
            lPlayer.path = lPath;
            lPlayer.MoveStepByStep();
        }

        public Vector2 GetPosFromIndex()
        {
            return GetPosFromIndex(Grid.IndexDict[this].X, Grid.IndexDict[this].Y, Grid);
        }

        /// <summary>
        /// Calculates a tile's position so that it is placed on the grid, with the grid's center at the center of the viewport.
        /// </summary>
        /// <param name="pIndexX"></param>
        /// <param name="pIndexY"></param>
        /// <param name="pGrid"></param>
        /// <returns></returns>
        private Vector2 GetPosFromIndex(int pIndexX, int pIndexY, Grid pGrid)
        {
            return MathS.IndexToPosition(SIZE, pIndexX, pIndexY) - 0.5f * MathS.PositionToIsoPosition((pGrid.Size - Vector2I.One) * SIZE);
        }

        public static Tile Create(int pIndexX, int pIndexY, Grid pGrid)
        {
            Tile lTile = NodeCreator.CreateNode<Tile>(GameManager.Instance.TileScene, pGrid);
            lTile.Grid = pGrid;
            lTile.Position = lTile.GetPosFromIndex(pIndexX, pIndexY, pGrid);
            return lTile;
        }
        

    }
}
