using Godot;
using System;

// Author : SMOLARSKI Camille

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim.AnimatedButtons
{
    public partial class ResumeButton : AnimatedButton
    {
        [Export] private Control menu;

        protected override void OnPressed()
        {
            base.OnPressed();
            menu.QueueFree();
        }
    }
}
