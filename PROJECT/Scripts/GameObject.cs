using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.Managers;
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

        protected bool shouldMove = false;
        private int durationLerp = 30;
        private int frameCount = 0;

        public override void _Ready()
        {
            base._Ready();
            GetWindow().SizeChanged += UpdatePos;
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            if (shouldMove) UpdatePos();

        }
        protected void UpdatePos()
        {
            frameCount++;
            float lProgress = (float)frameCount / durationLerp;
            if (lProgress > 1)
            {
                shouldMove = false;
                frameCount = 0;
                Player.GetInstance().MoveStepByStep();
                return;
            } 
            Position = Position.Lerp(CurrentTile.Position, lProgress);
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
