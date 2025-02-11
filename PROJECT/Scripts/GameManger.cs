using System;
using Godot;

namespace Com.IsartDigital.WoolyWay
{
    public partial class GameManger : Node
    {
        [ExportGroup("Nodes")]
        [ExportSubgroup("Containers")]
        [Export] public Node2D GameContainer { get; private set; }
        [ExportGroup("PackedScenes")]
        [ExportSubgroup("Grid")]
        [Export] public PackedScene TileScene { get; private set; }
        public static GameManger Instance { get; private set; }

        public override void _Ready()
        {
            if (Instance != null)
            {
                GD.Print("Error");
                QueueFree();
            }
            else
                Instance = this;
        }

        protected override void Dispose(bool pDisposing)
        {
            if (Instance == this)
                Instance = null;
            base.Dispose(pDisposing);
        }
    }
}
