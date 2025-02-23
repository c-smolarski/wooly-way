using Com.IsartDigital.WoolyWay.GameObjects;
using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay
{
    public partial class Tile : Node2D
    {
        public GameObject CurrentObject => MapManager.ObjectDict[this];
        public Vector2I Index => GridManager.TileDict[this];

        public bool IsWalkable()
        {
            return CurrentObject is not Obstacle || (CurrentObject is Sheep && ((Sheep)CurrentObject).CanMove());
        }

        public static Tile Create(Vector2 pPos)
        {
            return NodeCreator.CreateNode<Tile>(GameManger.Instance.TileScene, GameManger.Instance.GridContainer, pPos);
        }
    }
}
