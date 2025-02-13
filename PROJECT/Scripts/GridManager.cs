using Com.IsartDigital.WoolyWay.Scripts.Utils;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay
{
	
	public partial class GridManager : Node
	{
		#region Exports
		[ExportGroup("GridSettings")]
		[Export] private Vector2I tileOriginalSize;
		[Export] private Vector2 tileScale;
		[Export] private Vector2 tileMargin;
		[ExportGroup("PackedScenes")]
		[Export] private PackedScene tileScene;
		#endregion

		public const int MAX_GRID_SIZE = 11;

        public static GridManager Instance { get; private set; }

		private Vector2 TileSize => (tileOriginalSize + tileMargin) * tileScale;
		private Vector2 TileMargin
		{
			get => tileMargin;
			set
			{
				tileMargin = value;
				UpdateCurrentGridPos();
			}
		}

		public static Dictionary<Vector2I, Node2D> TileDict { get; private set; } = new();
		private static Vector2I? currentGridSize;

		public override void _Ready()
		{
            #region Singleton
            if (Instance != null)
			{
                GD.Print("Error : " + nameof(GridManager) + " already exists. The new one is being freed...");
				QueueFree();
				return;
            }
			Instance = this;
            #endregion
            GetWindow().SizeChanged += OnWindowSizeChange;
		}

        private void OnWindowSizeChange()
        {
			UpdateCurrentGridPos();
        }

        public void GenerateNewGrid(Vector2I pGridSize)
		{
			if (pGridSize.X > MAX_GRID_SIZE || pGridSize.Y > MAX_GRID_SIZE)
				throw new Exception($"Grid must not be higher than {MAX_GRID_SIZE}");

			DeleteCurrentGrid();
			currentGridSize = pGridSize;
            for (int i = 0; i < pGridSize.Y; i++)
				for (int j = 0; j < pGridSize.X; j++)
				{
					Node2D lTile = NodeCreator.CreateNode<Node2D>(
						tileScene,
						GameManger.Instance.GameContainer,
						GetTilePos(j, i)
					);
					lTile.Scale = tileScale;
					TileDict.Add(new Vector2I(j, i), lTile);
				}
			TileDict[new Vector2I(5, 5)].Modulate = Colors.Red;
        }

		private void DeleteCurrentGrid()
		{
			currentGridSize = null;
			foreach (Node lNode in TileDict.Values)
				lNode.QueueFree();
			TileDict.Clear();
		}

		private void UpdateCurrentGridPos()
		{
            foreach (KeyValuePair<Vector2I, Node2D> lPair in TileDict)
                lPair.Value.Position = GetTilePos(lPair.Key);
        }

		private Vector2 GetTilePos(Vector2I pIndex)
		{
			return GetTilePos(pIndex.X, pIndex.Y);
        }
        private Vector2 GetTilePos(int pIndexX, int pIndexY)
        {
            Vector2 lBorder = new Vector2(GetWindow().Size.X - tileOriginalSize.X * tileScale.X, GetWindow().Size.Y - MathS.IndexToPosition(TileSize, currentGridSize ?? Vector2I.Zero).Y) * 0.5f;
            return MathS.IndexToPosition(TileSize, pIndexX, pIndexY) + lBorder;
        }

        protected override void Dispose(bool pDisposing)
		{
			if (Instance == this)
				Instance = null;
		}
	}
}
