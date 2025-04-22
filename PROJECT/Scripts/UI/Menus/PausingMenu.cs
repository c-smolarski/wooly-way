using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

// Author : SMOLARSKI Camille

namespace Com.IsartDigital.WoolyWay.UI.Menus
{
    public abstract partial class PausingMenu : Control
    {
        public override void _Ready()
        {
            ProcessMode = ProcessModeEnum.Always;
            Displayed(true);
            TreeExiting += () => Displayed(false);

            SignalBus.Instance.FullscreenButtonPressed += ResetPos;
            GetViewport().SizeChanged += ResetPos;

            base._Ready();
        }

        private void ResetPos()
        {
            GlobalPosition = GetViewport().GetCamera2D().GlobalPosition;
        }

        private void Displayed(bool pDisplayed)
        {
            GetTree().Paused = pDisplayed;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.PausingMenuDisplayed);
        }

        protected override void Dispose(bool pDisposing)
        {
            SignalBus.Instance.FullscreenButtonPressed -= ResetPos;
            if (IsInstanceValid(GetViewport()))
                GetViewport().SizeChanged -= ResetPos;
            base.Dispose(pDisposing);
        }
    }
}