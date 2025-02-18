using Godot;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.Managers
{
    public partial class GameManger : Node
    {
        #region Exports
        [ExportGroup("Nodes")]
        [ExportSubgroup("Containers")]
        [Export] public Node2D GameContainer { get; private set; }
        #endregion

        public static GameManger Instance { get; private set; }

        public override void _Ready()
        {
            #region Singleton
            if (Instance != null)
            {
                GD.Print("Error : " + nameof(GameManger) + " already exists. The new one is being freed...");
                QueueFree();
                return;
            }
            Instance = this;
            #endregion
            GridManager.Instance.GenerateNewGrid(Vector2I.One * GridManager.MAX_GRID_SIZE);
        }

        protected override void Dispose(bool pDisposing)
        {
            if (Instance == this)
                Instance = null;
            base.Dispose(pDisposing);
        }
    }
}
