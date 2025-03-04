using Com.IsartDigital.WoolyWay;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils.TwoWayDictionnaries;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Com.IsartDigital.ProjectName
{
    public partial class Pathfinding : Node2D
    {
        Grid grid;
        public override void _Ready()
        {
        }

        //private float DistanceTo(Tile pTile, Tile pArrivalTile)
        //{
        //    float lDistance = MathF.Abs((GridManager.TileDict[pTile].X - GridManager.TileDict[pArrivalTile].X)) + MathF.Abs((GridManager.TileDict[pTile].Y - GridManager.TileDict[pArrivalTile].Y));
        //    return lDistance;
        //}

        //private void Path(Tile pStartPos, Tile pEndPos)
        //{
        //    List<Tile> lToSearch = new List<Tile>() { pStartPos };
        //    List<Tile> lProcessed = new List<Tile>();

        //    while (lToSearch.Any())
        //    {
        //        Tile lCurrent = lToSearch[0];
        //        foreach (Tile lTile in lToSearch)
        //        {
        //            float lDistance = DistanceTo(lTile, pEndPos);
        //            float lCurentDistance = DistanceTo(lCurrent, pStartPos);
        //            if (lDistance < lCurentDistance || lDistance == lCurentDistance && )

        //        }

        //    }
        //}

        //private List<Vector2> GetPath(ReadOnlyTwoWayDictionary<Vector2I, Tile> pIndexGrid, int pStartPosX, int pStartPosY, int pTargetPosX, int pTargetPosY)
        //{
            
        //}

        //private List<PathFindingCell> GetWalkableCell(ReadOnlyTwoWayDictionary<Vector2I, Tile> pIndexGrid, PathFindingCell pCurrentCell, PathFindingCell pTargetCell)
        //{
        //    List<PathFindingCell> possibleCells = new List<PathFindingCell>();
        //    if (pCurrentCell.posX < grid.Size.X-1 && pCurrentCell.posX> 0)
        //    {
        //        Tile lTile = grid.IndexDict[new Vector2I(pCurrentCell.posX, pCurrentCell.posY)];
        //        PathFindingCell lPossibleTile = new PathFindingCell();
        //        lPossibleTile.posX = pCurrentCell.posX - 1; 
        //    }



        //    return possibleCells;
        //}

        //private bool TestVisitedCell(PathFindingCell pWalkableCell, List<PathFindingCell> pVisitedCell)
        //{

        //}

        //private PathFindingCell IsActiveCellWalkable(PathFindingCell pWalkableCell, List<PathFindingCell> pActiveCells)
        //{

        //}
        public override void _Process(double delta)
        {

        }
    }
}