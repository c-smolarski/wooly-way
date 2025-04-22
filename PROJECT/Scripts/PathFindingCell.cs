using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.WoolyWay
{
	public partial class PathFindingCell : Node
	{
		public int posX;
		public int posY;
		public int cost;
		public int distance;
		public int costDistance;
		public PathFindingCell lastCell;

		public void SetDistance(int pTargetX, int pTargetY)
		{
			distance = Mathf.Abs(pTargetX - posX) + Mathf.Abs(pTargetY - posY);
		}
	}
}
