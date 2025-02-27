using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.GameObjects
{
    public abstract partial class Mobile : GameObject
    {


        protected void Move(Vector2I pMoveDirection)
        {
            Tile lCurrentTile = Grid.ObjectDict[this];
            GD.Print("Current tile: " + Grid.IndexDict[lCurrentTile]);
            Tile lNextTile = Grid.IndexDict[Grid.IndexDict[lCurrentTile] + pMoveDirection];
            GD.Print("next tile: " + Grid.IndexDict[lNextTile]);

            if (Grid.ObjectDict.Contains(lNextTile)) GD.Print(Grid.ObjectDict[lNextTile]);
            else GD.Print("Null");

            if (lNextTile.IsWalkable())
            {
                GD.Print("Condition Passed");
                Grid.ObjectDict[this] = lNextTile;
                GD.Print("New current tile: " + Grid.IndexDict[Grid.ObjectDict[this]]);
                UpdatePos();
            }
        }
        
    }
}
