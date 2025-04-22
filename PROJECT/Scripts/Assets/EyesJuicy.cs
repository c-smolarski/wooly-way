using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class EyesJuicy : Sprite2D
	{
		[Export] private Control center;
		[Export] private int rayon = 5;
		public override void _Ready()
		{
        }

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;
			EyesMove();
		}

		private void EyesMove()
		{
			GlobalPosition =  center.GlobalPosition + (GetGlobalMousePosition() - center.GlobalPosition).Normalized() * rayon;
		}
		protected override void Dispose(bool pDisposing)
		{

		}
	}
}
