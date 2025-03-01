using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class AuthentificationJuiciness : Node
	{
		[Export] private Label[] title;
		private Tween tween;

        public override void _Ready()
		{
			tween = CreateTween();
			JuicyTitle();
		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

		}

		private void JuicyTitle()
		{
			foreach (Label lLabel in title)
			{
				tween.TweenProperty(lLabel, "scale", Vector2.One, 0.4f).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
			}
		}
	}
}
