using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.UI.Buttons;
using Godot;
using System;

// Author : SMOLARSKI Camille

namespace Com.IsartDigital.WoolyWay.UI
{
    public partial class FullscreenButton : AbstractButton
    {
        public override void _Ready()
        {
            base._Ready();
            ButtonPressed = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen;
        }

        protected override void OnPressed()
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen ? DisplayServer.WindowMode.Windowed : DisplayServer.WindowMode.Fullscreen);
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.FullscreenButtonPressed);
        }
    }
}
