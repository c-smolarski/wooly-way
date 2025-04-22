using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using System;

// Author : SMOLARSKI Camille & Julien Lim

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim.AnimatedButtons.MenuButtons
{
	public partial class RestartButton : ResumeButton
	{
        protected override void OnPressed()
		{
			base.OnPressed();
			SignalBus.Instance.EmitSignal(SignalBus.SignalName.LevelRestarted);
			GD.Print("Level restarted");
		}
	}
}
