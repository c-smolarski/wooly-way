using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class AuthentificationJuiciness : Node
	{
		[Export] private Label[] title;
		private Tween tween;
		private const string TWEEN_SCALE = "scale";
		private const float TWEEN_DURATION = 0.4f;
        public override void _Ready()
		{
			tween = CreateTween();
			JuicyTitle();
		}

		private void JuicyTitle()
		{
			foreach (Label lLabel in title)
			{
				tween.TweenProperty(lLabel, TWEEN_SCALE, Vector2.One, TWEEN_DURATION).SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
			}
		}
	}
}
