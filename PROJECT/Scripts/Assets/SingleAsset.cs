using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.Scripts.Assets
{
    public abstract partial class SingleAsset : Asset
    {
        protected Vector2 BaseScale { get; private set; }

        public override void _Ready()
        {
            base._Ready();
            BaseScale = Scale;
            ResetValues();
        }

        public override void PlayAppearAnim()
        {
            Visible = true;
        }

        protected virtual void ResetValues()
        {
            Scale = BaseScale;
            Visible = false;
        }
    }
}
