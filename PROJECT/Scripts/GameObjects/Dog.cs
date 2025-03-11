using Godot;


// Author : Tom Bagnara

namespace Com.IsartDigital.WoolyWay.GameObjects
{
	public partial class Dog : Obstacle
	{
		public Vector2I Direction { get; private set; }


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
			return lDog;
		}
	}
}
