using Com.IsartDigital.WoolyWay.GameObjects;
using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

// Author : Camille SMOLARSKI & Tom BAGNARA

namespace Com.IsartDigital.WoolyWay
{
    public partial class Tile : Node2D
    {
        [Export] private Vector2 size;

        public Grid Grid { get; private set; }
        public GameObject CurrentObject => Grid.ObjectDict[this];

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
        /// Calculates a tile's position so that it is placed on the grid, with the grid's center at the center of the viewport.
        /// </summary>
        /// <param name="pIndexX"></param>
        /// <param name="pIndexY"></param>
        /// <param name="pGrid"></param>
        /// <returns></returns>
        private Vector2 GetPosFromIndex(int pIndexX, int pIndexY, Grid pGrid)
        {
            Vector2 scaledSize = size * Scale;
            return MathS.IndexToPosition(scaledSize, pIndexX, pIndexY) - 0.5f * MathS.PositionToIsoPosition((pGrid.Size - Vector2I.One) * scaledSize);
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
