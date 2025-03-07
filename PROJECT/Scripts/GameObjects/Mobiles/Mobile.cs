using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Godot;
using System;
using System.Collections.Generic;

// Author : Tom BAGNARA

namespace Com.IsartDigital.WoolyWay.GameObjects
{
    public abstract partial class Mobile : GameObject
    {


        /// <summary>
        /// Checks if current object can move in the specified direction by verifying if the next tile is walkable.
        /// If the next tile contains a sheep and is not blocked by another object, it moves it out of the way.
        /// </summary>
        /// <param name="pMoveDirection"></param>
        public virtual void InitMove(Vector2I pMoveDirection)
        {
            Tile lNextTile = Grid.IndexDict[Grid.IndexDict[CurrentTile] + pMoveDirection];

            if (!lNextTile.IsWalkable(pMoveDirection)) return;

            if (!lNextTile.IsEmpty() && lNextTile.IsSheep())
            { 
                Sheep lSheep = Grid.ObjectDict[lNextTile] as Sheep;
                    
                Move(lNextTile);

                lSheep?.InitMove(pMoveDirection);
            }
            else Move(lNextTile);
        }
        
        protected void Move(Tile pTile)
        {
            Grid.ObjectDict[this] = pTile;
            UpdatePos();
        }
    }
}
