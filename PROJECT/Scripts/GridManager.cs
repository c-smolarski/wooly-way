using Com.IsartDigital.WoolyWay.Utils.TwoWayDictionnaries;
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

		public static ReadOnlyTwoWayDictionary<Vector2I, Tile> TileDict { get; private set; } = new();
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
			TwoWayDictionary<Vector2I, Tile> lDict = new();
			currentGridSize = pGridSize;
            for (int i = 0; i < pGridSize.Y; i++)
				for (int j = 0; j < pGridSize.X; j++)
				{
					Tile lTile = NodeCreator.CreateNode<Tile>(
						tileScene,
						GameManger.Instance.GameContainer,
						GetTilePos(j, i)
					);
					lTile.Scale = tileScale;
					lDict.Add(new Vector2I(j, i), lTile);
				}
			TileDict = lDict.ToReadOnly();
        }

		private void DeleteCurrentGrid()
		{
			currentGridSize = null;
			foreach (Node lNode in TileDict.Values)
				lNode.QueueFree();
			TileDict = null;
		}

		/// <summary>
		/// Recalculates each tile's position so that the center of the grid will be at the center of the window.
		/// </summary>
		private void UpdateCurrentGridPos()
		{
            foreach (KeyValuePair<Vector2I, Tile> lPair in TileDict)
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
        private Vector2 GetTilePos(Tile pNode)
        {
            return GetTilePos(TileDict[pNode]);
        }

        /// <summary>
        /// DiagonalDirection is based off a standard (Non-isometric) table. Max value on grid for pIndex is (Max(GridSize.X, GridSize.Y) - 1) * 2.
        /// <para>Ex : For GetTilesDiagonally(BottomRight, 2) it will return tiles at index (0,2), (1,1) and (2,0).</para>
		/// </summary>
        /// <param name="pDirection"></param>
        /// <param name="pIndex"></param>
        /// <returns>A list of diagonally adjacent tiles at specified index on grid.</returns>
        /// <exception cref="Exception"></exception>
        public static List<Tile> GetTilesDiagonally(DiagonalDirection pDirection, int pIndex)
		{
			if (currentGridSize == default || currentGridSize == null)
				throw new Exception("A Grid must be present.");

			List<Tile> lTileList = new();
			Func<Vector2I, int> lAction = default;
			Vector2I lGridMaxIndex = (currentGridSize ?? default) - Vector2I.One;

            switch (pDirection)
			{
				case DiagonalDirection.TOP_RIGHT:
					lAction = pVector => pVector.X - pVector.Y + lGridMaxIndex.Y;
					break;
				case DiagonalDirection.TOP_LEFT:
					lAction = pVector => (lGridMaxIndex.X - pVector.X) + (lGridMaxIndex.Y - pVector.Y);
					break;
				case DiagonalDirection.BOTTOM_LEFT:
					lAction = pVector => pVector.Y - pVector.X + lGridMaxIndex.X;
					break;
				case DiagonalDirection.BOTTOM_RIGHT:
					lAction = pVector => pVector.Y - (lGridMaxIndex.X - pVector.X) + lGridMaxIndex.X;
					break;
			}

			foreach(Vector2I lIndex in TileDict.Keys)
				if (lAction(lIndex) == pIndex)
					lTileList.Add(TileDict[lIndex]);

			return lTileList;
		}

        protected override void Dispose(bool pDisposing)
		{
			if (Instance == this)
				Instance = null;
		}
	}
}
