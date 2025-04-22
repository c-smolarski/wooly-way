using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.WoolyWay
{
	public partial class AuthentificationJuiciness : Control
	{
		[Export] private Label[] title;
		private Tween tween;
		private const string TWEEN_SCALE = "scale";
		private const float TWEEN_DURATION = .4f;
        public override void _Ready()
		{
			tween = CreateTween();
			JuicyTitle();
		}

		private void JuicyTitle()
		{
			foreach (Label label in title)
			{
				label.Scale = Vector2.Zero;
				tween.TweenProperty(label, TWEEN_SCALE, Vector2.One, TWEEN_DURATION)
					.SetTrans(Tween.TransitionType.Back)
					.SetEase(Tween.EaseType.Out);
			}
		}
	}
}
