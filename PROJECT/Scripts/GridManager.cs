using Com.IsartDigital.WoolyWay.Utils.TwoWayDictionnaries;
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
		#region Exports
		[ExportGroup("GridSettings")]
		[Export] private Vector2I tileOriginalSize;
		[Export] private Vector2 tileScale;
		[Export] private Vector2 tileMargin;
		[ExportGroup("PackedScenes")]
		[Export] private PackedScene tileScene;
		#endregion

		//Max grid size imposed by GDD.
		public const int MAX_GRID_SIZE = 11;

        public static GridManager Instance { get; private set; }

		/// <summary>
		/// General tile size accounitg scale and margin between tiles.
		/// </summary>
		private Vector2 TileSize => (tileOriginalSize + tileMargin) * tileScale;

        /// <summary>
        /// Defines the margin (Little spaces) between each tile. Probably for testing only.
        /// </summary>
        private Vector2 TileMargin
		{
			get => tileMargin;
			set
			{
				tileMargin = value;
				UpdateCurrentGridPos();
			}
		}

		public static ReadOnlyTwoWayDictionary<Vector2I, Node2D> TileDict { get; private set; } = new();
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

		//Repositions the grid at window's center when size is changed at runtime.
        private void OnWindowSizeChange()
        {
			UpdateCurrentGridPos();
        }

		/// <summary>
		/// Generates a new isometric grid and displays tiles accordingly. If a previous grid existed it will be deleted.
		/// Grid size must NOT be greater than MAX_GRID_SIZE
		/// </summary>
		/// <param name="pGridSize"></param>
		/// <exception cref="Exception"></exception>
        public void GenerateNewGrid(Vector2I pGridSize)
		{
			if (pGridSize.X > MAX_GRID_SIZE || pGridSize.Y > MAX_GRID_SIZE)
				throw new Exception($"Grid must not be higher than {MAX_GRID_SIZE}");

			DeleteCurrentGrid();
			TwoWayDictionary<Vector2I, Node2D> lDict = new();
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
					lDict.Add(new Vector2I(j, i), lTile);
				}
			TileDict = lDict.ToReadOnly();
			TileDict[new Vector2I(5, 5)].Modulate = Colors.Red; //For testing only.
        }

		private void DeleteCurrentGrid()
		{
			currentGridSize = null;
			foreach (Node lNode in TileDict.Values)
				lNode.QueueFree();
			TileDict = new();
		}

		/// <summary>
		/// Recalculates each tile's position so that the center of the grid will be at the center of the window.
		/// </summary>
		private void UpdateCurrentGridPos()
		{
            foreach (KeyValuePair<Vector2I, Node2D> lPair in TileDict)
                lPair.Value.Position = GetTilePos(lPair.Key);
        }

		/// <summary>
		/// Calculates a tile's position so that it is placed on the grid, with the grid's center at the center of window.
		/// </summary>
		/// <param name="pIndexX"></param>
		/// <param name="pIndexY"></param>
		/// <returns></returns>
        private Vector2 GetTilePos(int pIndexX, int pIndexY)
        {
            Vector2 lBorder = new Vector2(GetWindow().Size.X - tileOriginalSize.X * tileScale.X, GetWindow().Size.Y - MathS.IndexToPosition(TileSize, currentGridSize ?? Vector2I.Zero).Y) * 0.5f;
            return MathS.IndexToPosition(TileSize, pIndexX, pIndexY) + lBorder;
        }

        /// <summary>
        /// Calculates a tile's position so that it is placed on the grid, with the grid's center at the center of window.
        /// </summary>
        /// <param name="pIndex"></param>
        /// <returns></returns>
        private Vector2 GetTilePos(Vector2I pIndex)
        {
            return GetTilePos(pIndex.X, pIndex.Y);
        }

        /// <summary>
		/// Calculates a tile's position so that it is placed on the grid, with the grid's center at the center of window.
        /// </summary>
        /// <param name="pNode"></param>
        /// <returns></returns>
        private Vector2 GetTilePos(Node2D pNode)
        {
            return GetTilePos(TileDict[pNode]);
        }

        protected override void Dispose(bool pDisposing)
		{
			if (Instance == this)
				Instance = null;
		}
	}
}
