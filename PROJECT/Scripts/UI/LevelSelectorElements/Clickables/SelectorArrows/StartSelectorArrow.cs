using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables.SelectorArrows
{
    public partial class StartSelectorArrow : SelectorArrow
    {

        protected override void OnClick()
        {
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.DisplayLevelButton, 1);
        }
    }
}
