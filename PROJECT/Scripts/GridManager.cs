using Com.IsartDigital.WoolyWay.Scripts.Utils;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;
using System.Collections.Generic;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay
{
	
	public partial class GridManager : Node
	{
		[ExportGroup("GridSettings")]
		[Export] public Vector2 GridSize { get; private set; }
		[Export] public Vector2 TileSize { get; private set; }
		[Export] public Vector2 TileScale { get; private set; }
		[Export] public float MarginBetweenTiles { get; private set; }


		public static GridManager Instance { get; private set; }
		public Dictionary<Vector2I, Node2D> TileDict { get; private set; } = new();

		public override void _Ready()
		{
			if (Instance != null)
			{
                GD.Print("Error");
				QueueFree();
            }
			else
				Instance = this;
			GenerateGrid(GridSize);
		}

		private void GenerateGrid(Vector2 pGridSize)
		{
			for (int i = 0; i < pGridSize.Y; i++)
				for (int j = 0; j < pGridSize.X; j++)
				{
					Node2D lTile = NodeCreator.CreateNode<Node2D>(
						GameManger.Instance.TileScene,
						GameManger.Instance.GameContainer,
						MathS.IndexToPosition(j, i) - Vector2.Right * TileSize * TileScale * 0.5f + Vector2.Right * GetWindow().Size * 0.5f
					);
					lTile.Scale = TileScale;
					TileDict.Add(new Vector2I(j, i), lTile);
				}
        }

		protected override void Dispose(bool pDisposing)
		{
			if (Instance == this)
				Instance = null;
		}
	}
}
