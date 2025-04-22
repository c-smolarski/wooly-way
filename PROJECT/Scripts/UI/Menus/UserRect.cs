using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class UserRect : ColorRect
	{
		private Color baseColor;
		[Export] private Color focusedColor;
		public override void _Ready()
		{
			baseColor = Modulate;
			MouseEntered += ChangeColor;
			MouseExited += ChangeColor;
		}

		private void ChangeColor()
		{
			if(Modulate == baseColor) Modulate = focusedColor;
			else Modulate = baseColor;
		}
		protected override void Dispose(bool pDisposing)
		{

		}
	}
}
