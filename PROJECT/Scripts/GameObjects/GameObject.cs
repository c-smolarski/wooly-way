using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

// Author : Camille SMOLARSKI && Alissa DELATTRE

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

        public void UpdatePos()
        {
            Position = CurrentTile.Position;
            SetZIndex(CurrentTile);
        }

        protected void SetZIndex(Tile pTile)
        {
            ZIndex = pTile.ZIndex + pTile.GetDiagonalIndex(DiagonalDirection.BOTTOM_RIGHT);
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
            lObj.SetZIndex(pTile);
            return lObj;
        }
    }
}
