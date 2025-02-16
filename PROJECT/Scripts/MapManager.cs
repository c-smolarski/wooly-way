using Godot;
using System;
using System.IO;
using System.Text.Json;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName {
	
	public partial class MapManager : Node
	{
		public int current_level;
        private string pathJson = "Properties/leveldesign.json";
        public override void _Ready()
		{
			ExtractData();
		}

		public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

		}

		private void ExtractData()
		{
			string data = File.ReadAllText(pathJson);
			try
			{
				MapData mapData = JsonSerializer.Deserialize<MapData>(data)!;
				GD.Print(mapData.level1.author);
            }
            catch (Exception e)
            {
                GD.PrintErr(e.Message);
            }
			
        }

		protected override void Dispose(bool pDisposing)
		{

		}
	}
}
