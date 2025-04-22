using Godot;
using System;

// Author : SMOLARSKI Camille & Julien Lim

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim.AnimatedButtons
{
	public partial class QuitButton : AnimatedButton
	{
        protected override void OnPressed()
        {
			base.OnPressed();
			GetTree().Quit();
        }
    }
}
