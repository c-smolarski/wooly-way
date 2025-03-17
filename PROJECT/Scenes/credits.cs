using Godot;
using System;

// Author : Julien Lim

namespace Com.IsartDigital.WoolyWay
{
	
	public partial class credits : Control
	{
        [Export] public Button BackButton { get; set; }
        public override void _Ready()
		{
            BackButton.Pressed += BackButton_Pressed;
        }

        private void BackButton_Pressed()
        {
            QueueFree();
        }

	}
}
