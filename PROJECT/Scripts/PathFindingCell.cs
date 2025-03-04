using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class PathFindingCell : Node
	{
		public int posX;
		public int posY;
		int cost;
		int distance;
		int costDistance;
		PathFindingCell lastCell;
		public override void _Ready()
		{

		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

		}

		private void SetDistance(int pTargetX, int pTargetY)
		{
			distance = Mathf.Abs(pTargetX - posX) + Mathf.Abs(pTargetY - posY);
		}

		protected override void Dispose(bool pDisposing)
		{

		}
	}
}
