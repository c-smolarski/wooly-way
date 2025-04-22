using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class lineEdit : LineEdit
	{
        [Export] ColorRect rect;
        [Export] private Color focusedColor;

        private Color baseColor;

        public override void _Ready()
		{
            baseColor = rect.Modulate;
            MouseEntered += ChangeColor;
            MouseExited += ChangeColor;
        }

        private void ChangeColor()
        {
            if (rect.Modulate == baseColor) rect.Modulate = focusedColor;
            else rect.Modulate = baseColor;
        }
	}
}
