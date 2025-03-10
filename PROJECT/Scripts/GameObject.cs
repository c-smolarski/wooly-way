using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay
{
    public partial class GameObject : Node2D
    {
        public Grid Grid { get; private set; }
        public Tile CurrentTile => Grid.ObjectDict[this];

        public override void _Ready()
        {
            base._Ready();
            GetWindow().SizeChanged += UpdatePos;
        }

        private void UpdatePos()
        {
            Position = CurrentTile.Position;
        }

        // Initialization methods.
        public static GameObject Create(PackedScene pScene, Tile pTile)
        {
            return Create<GameObject>(pScene, pTile);
        }

        public static T Create<T>(PackedScene pScene, Tile pTile) where T : GameObject
        {
            T lObj = NodeCreator.CreateNode<T>(pScene, pTile.Grid ,pTile.Position);
            lObj.Grid = pTile.Grid;
            return lObj;
        }
    }
}
