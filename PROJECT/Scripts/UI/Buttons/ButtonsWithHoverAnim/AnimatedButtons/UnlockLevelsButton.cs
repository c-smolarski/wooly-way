using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

// Author : Daniel Moussouni--Lepilliez

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim.AnimatedButtons
{
    public partial class UnlockLevelsButton : AnimatedButton
    {
        private const int MAX_LEVELS = 6;
        protected override void OnPressed()
        {
            base.OnPressed();
            SignalBus.Instance.EmitSignal(nameof(SignalBus.SignalName.UnlockLevel), MAX_LEVELS);
        }
    }
}