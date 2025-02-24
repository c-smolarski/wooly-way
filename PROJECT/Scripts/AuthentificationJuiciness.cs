using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class AuthentificationJuiciness : Node
	{
		[Export] private AnimationPlayer wooly;
		[Export] private AnimationPlayer way;
		public override void _Ready()
		{
			startAnim();
		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

		}

		private void startAnim()
		{
			// DU DURE TJ PLUS DE DURE
			wooly.Play("Title");
			wooly.AnimationFinished += EndAnim;
		}

        private void EndAnim(StringName animName)
		{
			//AAAAAAAAAAAAAAH
			way.Play("Way");
		}
		protected override void Dispose(bool pDisposing)
		{

		}
	}
}
