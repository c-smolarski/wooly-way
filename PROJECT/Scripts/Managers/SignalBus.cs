using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.Managers
{
    public partial class SignalBus : Node
    {
        [Signal] public delegate void DisplayLevelButtonEventHandler(int pLevelNumber);

        public static SignalBus Instance { get; private set; }

        public override void _Ready()
        {
            #region Singleton
            if (Instance != null)
            {
                GD.Print("Error : " + nameof(SignalBus) + " already exists. The new one is being freed...");
                QueueFree();
                return;
            }
            Instance = this;
            #endregion
        }

        protected override void Dispose(bool pDisposing)
        {
            if (Instance == this)
                Instance = null;
            base.Dispose(pDisposing);
        }
    }
}
