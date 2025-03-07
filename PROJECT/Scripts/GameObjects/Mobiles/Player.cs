using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;
using System.Collections.Generic;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.GameObjects.Mobiles
{
    public partial class Player : Mobile
    {
        public override void _Ready()
        {
            InputManager.Instance.MoveInputPressed += InitMove;
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }

        protected override void Dispose(bool pDisposing)
        {
            base.Dispose(pDisposing);
        }
    }
}
