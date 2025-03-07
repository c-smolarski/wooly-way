using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// Author : Camille SMOLARSKI && Alissa DELATTRE

namespace Com.IsartDigital.WoolyWay.GameObjects.Mobiles
{
    public partial class Player : Mobile
    {
        #region Singleton
        static private Player instance;
        private Player() { }

        static public Player GetInstance()
        {
            if (instance == null) instance = new Player();
            return instance;
        }

        #endregion
        public override void _Ready()
        {
            #region Singleton
            if (instance != null)
            {
                QueueFree();
                GD.Print(nameof(Player) + "Instance already exists, destroying the last added");
                return;
            }

            instance = this;
            #endregion

            InputManager.Instance.MoveInputPressed += InitMove;
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }

        /// <summary>
       ///Moves the player tile by tile until he reaches the target
        /// </summary>
        public void MoveStepByStep(List<Vector2I> pWay)
        {
            for (int i = pWay.Count-1; i >= 0; i--)
            {
                InitMove(pWay[i] - Grid.IndexDict[Grid.ObjectDict[this]]);
                pWay.Remove(pWay[i]);
            }
        }
        protected override void Dispose(bool pDisposing)
        {
            if (instance == this) instance = null;
            base.Dispose(pDisposing);
        }
    }
}
