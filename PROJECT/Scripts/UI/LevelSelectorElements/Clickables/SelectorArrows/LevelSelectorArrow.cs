using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables.SelectorArrows
{
    public partial class LevelSelectorArrow : SelectorArrow
    {
        [Export] private LevelButton level;
        [Export] private bool next;

        public override void _Ready()
        {
            base._Ready();

            ChangeVisibilty(false);
            level.Focused += ChangeVisibilty;
        }

        protected override void OnClick()
        {
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.DisplayLevelButton, level.LevelNumber + (next ? 1 : -1));
        }

        protected override void Dispose(bool pDisposing)
        {
            level.Focused -= ChangeVisibilty;
            base.Dispose(pDisposing);
        }
    }
}
