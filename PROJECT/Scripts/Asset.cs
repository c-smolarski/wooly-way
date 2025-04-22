using Godot;
using System;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay
{
    public abstract partial class Asset : Node2D
    {
        protected abstract float AnimStepDuration { get; } 

        public abstract void PlayAppearAnim();
        public abstract void PlayDisappearAnim();
    }
}
