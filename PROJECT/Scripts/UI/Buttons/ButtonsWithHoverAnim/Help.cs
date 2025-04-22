using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim
{
	public partial class Help : ButtonWithHoverAnim
	{
		[Export] private Sprite2D helpMenu;
		[Export] private Button closeMenu;
		public override void _Ready()
		{
			base._Ready();
			closeMenu.Pressed += CloseHelp;
		}

		private void CloseHelp()
		{
			helpMenu.Visible = false;
		}
        protected override void OnPressed()
        {
            base.OnPressed();
            helpMenu.Visible = true;
        }
	}
}
