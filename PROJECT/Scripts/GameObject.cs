using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay
{
    public partial class GameObject : Node2D
    {
        public Tile CurrentTile => MapManager.ObjectDict[this];


        public override void _Ready()
        {
            base._Ready();
            GetWindow().SizeChanged += UpdatePos;
        }

        private void UpdatePos()
        {
            GD.Print(CurrentTile);
            Position = CurrentTile.Position;
        }

        // Initialization methods.
        public static GameObject Create(PackedScene pScene, Tile pTile)
        {
            return Create<GameObject>(pScene, pTile);
        }
        public static GameObject Create(PackedScene pScene, int pIndexX, int pIndexY)
        {
            return Create<GameObject>(pScene, pIndexX, pIndexY);
        }

        public static T Create<T>(PackedScene pScene, Tile pTile) where T : GameObject
        {
            return NodeCreator.CreateNode<T>(pScene, GameManger.Instance.GameContainer, pTile.Position);
        }
        public static T Create<T>(PackedScene pScene, Vector2I pTileIndex) where T : GameObject
        {
            return Create<T>(pScene, GridManager.TileDict[pTileIndex]);
        }
        public static T Create<T>(PackedScene pScene, int pIndexX, int pIndexY) where T : GameObject
        {
            return Create<T>(pScene, GridManager.TileDict[new Vector2I(pIndexX, pIndexY)]);
        }
    }
}
