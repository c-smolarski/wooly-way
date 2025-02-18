using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay
{
    public partial class Tile : Node2D
    {
        public static Tile Create(Vector2 pPos)
        {
            return NodeCreator.CreateNode<Tile>(GameManger.Instance.TileScene, GameManger.Instance.GridContainer, pPos);
        }
    }
}
