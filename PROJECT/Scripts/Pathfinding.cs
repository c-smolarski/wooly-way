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
            grid = LevelManager.currentLevel;
        }

        private List<Vector2> GetPath(int pStartPosX, int pStartPosY, int pTargetPosX, int pTargetPosY)
        {
            List<Vector2> lPath = new List<Vector2>();
            PathFindingCell lStartCell = new PathFindingCell { posX = pStartPosX, posY = pStartPosY };
            PathFindingCell lTargetCell = new PathFindingCell { posX = pTargetPosX, posY = pTargetPosY };
            lStartCell.SetDistance(lTargetCell.posX, lTargetCell.posY);

            List<PathFindingCell> lActiveCells = new List<PathFindingCell>();
            lActiveCells.Add(lStartCell);
            List<PathFindingCell> lVisitedCells = new List<PathFindingCell>();
            while (lActiveCells.Count > 0)
            {
                lActiveCells.Sort((x, y) => x.costDistance.CompareTo(y.costDistance));
                PathFindingCell lCheckCell = lActiveCells[0];

                if (lCheckCell.posX == lTargetCell.posX && lCheckCell.posY == lTargetCell.posY)
                {
                    PathFindingCell lCell = lCheckCell;
                    while (true)
                    {
                        lPath.Add(new Vector2(lCell.posX, lCell.posY));
                        lCell = lCell.lastCell;

                        if (lCell == null) return lPath;
                    }
                }

                lVisitedCells.Add(lCheckCell);
                lActiveCells.Remove(lCheckCell);

                List<PathFindingCell> lWalkableCells = GetWalkableCell(lCheckCell, lTargetCell);

                for (int i = 0; i < lWalkableCells.Count; i++)
                {
                    if (TestVisitedCell(lWalkableCells[i], lVisitedCells)) continue;


                    if (IsActiveCellWalkable(lWalkableCells[i], lActiveCells))
                    {
                        PathFindingCell lExistingCell = default;
                        for (int k = 0; k < lActiveCells.Count; k++)
                        {
                            if (lActiveCells[k].posX == lWalkableCells[i].posX && lActiveCells[k].posY == lWalkableCells[i].posY)
                            {
                                lExistingCell = lActiveCells[k];
                                break;
                            }
                        }

                        if (lExistingCell.costDistance > lCheckCell.costDistance)
                        {
                            lActiveCells.Remove(lExistingCell);
                            lActiveCells.Add(lWalkableCells[i]);
                        }

                    }
                    else lActiveCells.Add(lWalkableCells[i]);
                }
            }
            return lPath;
        }

        public List<PathFindingCell> GetWalkableCell(PathFindingCell pCurrentCell, PathFindingCell pTargetCell)
        {
            List<PathFindingCell> lWalkableCell = new List<PathFindingCell>();
            List<Tile> lPossibleCells = LevelManager.currentLevel.Neighbors(LevelManager.currentLevel.IndexDict[new Vector2I(pCurrentCell.posX, pCurrentCell.posY)]);
            int lNumPossibleCells = lPossibleCells.Count;


            for (int i = 0; i < lNumPossibleCells; i++)
            {
                if (!LevelManager.currentLevel.ObjectDict.Contains(lPossibleCells[i]))
                {
                    lWalkableCell.Add(new PathFindingCell { posX = LevelManager.currentLevel.IndexDict[lPossibleCells[i]].X, posY = LevelManager.currentLevel.IndexDict[lPossibleCells[i]].Y, lastCell = pCurrentCell });
                }
            }
            lWalkableCell.ForEach(lCell => lCell.SetDistance(pTargetCell.posX, pTargetCell.posY));
            return lWalkableCell;
        }

        private bool TestVisitedCell(PathFindingCell pWalkableCell, List<PathFindingCell> pVisitedCell)
        {
            for (int i = 0; i < pVisitedCell.Count; i++)
            {
                if (pWalkableCell.posX == pVisitedCell[i].posX && pWalkableCell.posY == pVisitedCell[i].posY)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsActiveCellWalkable(PathFindingCell pWalkableCell, List<PathFindingCell> pActiveCells)
        {
            for (int i = 0; i < pActiveCells.Count; i++)
            {
                if (pWalkableCell.posX == pActiveCells[i].posX && pWalkableCell.posY == pActiveCells[i].posY)
                {
                    return true;
                }
            }
            return false;
        }

        public override void _Process(double delta)
        {

        }
    }
}