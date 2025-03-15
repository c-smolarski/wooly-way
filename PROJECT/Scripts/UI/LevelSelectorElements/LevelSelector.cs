using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements
{
    public partial class LevelSelector : Node2D
    {
        [Export(PropertyHint.Range, "1, 100, 1")] public int WorldNumber { get; private set; } = 1;

        public override void _Ready()
        {
            base._Ready();
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.UnlockLevel, 6);

        }
    }
}
