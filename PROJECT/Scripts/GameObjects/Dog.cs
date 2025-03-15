using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;


// Author : Tom Bagnara

namespace Com.IsartDigital.WoolyWay.GameObjects
{
	public partial class Dog : Obstacle
	{
		private const string IDLE_BACK = "IdleBack";
		private const string IDLE_FRONT = "IdleFront";
		
		private Vector2I Direction { get; set; }
		
		[Export] private AnimatedSprite2D Renderer;

		
		public override void _Ready()
		{
			base._Ready();
	        
			Tile lTargetTile = Grid.IndexDict[Grid.IndexDict[CurrentTile] + Direction];

			lTargetTile?.SetDog(true);
		}

		
    
		public static Dog Create(PackedScene pScene, Tile pTile, Vector2I pDirection)
		{
			Dog lDog = Create<Dog>(pScene, pTile);
			lDog.Direction = pDirection.Sign();
			lDog.SetSpriteDirection();
			return lDog;
		}

		private void SetSpriteDirection()
		{
			switch (Direction)
			{
				case (-1,0):
					Renderer.Animation = IDLE_BACK;
					Renderer.FlipH = false;
					break;
				case (1, 0):
					Renderer.Animation = IDLE_FRONT;
					Renderer.FlipH = true;
					break;
				case (0, 1):
					Renderer.Animation = IDLE_FRONT;
					Renderer.FlipH = false;
					break;
				case (0, -1):
					Renderer.Animation = IDLE_BACK;
					Renderer.FlipH = true;
					break;
			}
		}
	}
}
