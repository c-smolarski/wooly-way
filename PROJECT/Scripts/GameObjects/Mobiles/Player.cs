using Godot;
using System;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.GameObjects.Mobiles
{
    public partial class Player : Mobile
    {
        public static Player Instance { get; private set; }

        public override void _Ready()
        {
            #region Singleton
            if (Instance != null)
            {
                GD.Print("Error : " + nameof(Player) + " already exists. The new one is being freed...");
                QueueFree();
                return;
            }
            Instance = this;
            #endregion
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }

        protected override void Dispose(bool pDisposing)
        {
            if (Instance == this)
                Instance = null;
            base.Dispose(pDisposing);
        }
    }
}
