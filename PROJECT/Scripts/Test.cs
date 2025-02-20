using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class Test : Polygon2D
	{
		[Export] private AnimationPlayer transition;
		public override void _Ready()
		{
			transition.Play("fadeOut");
		}
	}
}
