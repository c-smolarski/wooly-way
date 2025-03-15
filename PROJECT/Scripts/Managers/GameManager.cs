using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.Managers
{
    public partial class GameManager : Node
    {
        #region Exports
        [ExportGroup("Nodes")]
        [ExportSubgroup("Containers")]
        [Export] public Node2D GameContainer { get; private set; }
        [ExportGroup("PackedScenes")]
        [Export] public PackedScene TileScene { get; private set; }
        [Export] public PackedScene LevelButtonScene { get; private set; }
        [ExportSubgroup("GameObjects")]
        [Export] public PackedScene WallScene { get; private set; }
        [Export] public PackedScene PlayerScene { get; private set; }
        [Export] public PackedScene DogScene { get; private set; }
        [Export] public PackedScene SheepScene { get; private set; }
        [Export] public PackedScene TargetScene { get; private set; }
        #endregion

        MapData mapData = new MapData();
        LevelManager levelManager;
        public MapInfo currentLevelInfos;

        public static GameManager Instance { get; private set; }

        public override void _Ready()
        {
            #region Singleton
            if (Instance != null)
            {
                GD.Print("Error : " + nameof(GameManager) + " already exists. The new one is being freed...");
                QueueFree();
                return;
            }
            Instance = this;
            #endregion
            levelManager = LevelManager.GetInstance();
            currentLevelInfos = LevelManager.GetInstance().GenerateLevel(LevelManager.MapData.Worlds["World1"]["Level1"]);
            HudManager.Instance.CreateHud();
        }

        protected override void Dispose(bool pDisposing)
        {
            if (Instance == this)
                Instance = null;
            base.Dispose(pDisposing);
        }
    }
}
