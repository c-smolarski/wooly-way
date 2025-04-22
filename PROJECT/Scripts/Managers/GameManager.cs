using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim.MainButtons;
using Com.IsartDigital.WoolyWay.Utils;
using Com.IsartDigital.WoolyWay.Data;
using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.Managers
{
    public partial class GameManager : Node
    {
        #region Exports
        [ExportGroup("Nodes")]
        [ExportSubgroup("Containers")]
        [Export] public Node2D AssetsContainer { get; private set; }
        [Export] public Node2D LevelSelectorContainer { get; private set; }
        [Export] public Node2D GameContainer { get; private set; }
        [Export] public Node2D TutorialContainer { get; private set; }
        [Export] private Node2D menusContainer;
        [Export] public CanvasLayer canvasLayer;
        [Export] public Camera2D camera;
        [Export] public CloseTutorial buttonTuto;

        [ExportGroup("PackedScenes")]
        [Export] public PackedScene TileScene { get; private set; }
        [Export] private PackedScene winScreenPacked;
        [Export] private PackedScene pauseScreenPacked;
        [Export] public PackedScene history;
        [ExportSubgroup("GameObjects")]
        [Export] public PackedScene WallScene { get; private set; }
        [Export] public PackedScene PlayerScene { get; private set; }
        [Export] public PackedScene DogScene { get; private set; }
        [Export] public PackedScene SheepScene { get; private set; }
        [Export] public PackedScene TargetScene { get; private set; }
        #endregion

        private bool IsTutoOpened = false;
        public static GameManager Instance { get; private set; }

        public static Player.ControlScheme currentControlScheme = Player.ControlScheme.CLASSIC;

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


        }

        public async void Opentuto()
        {

            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
            if(IsTutoOpened) return;
            buttonTuto.OpenTuto();
            IsTutoOpened=true;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            if (!LevelManager.isTutorial && Input.IsActionJustPressed(InputManager.MENU_PAUSE))
                LoadPauseMenu();
        }

        public void LoadPauseMenu()
        {
            NodeCreator.CreateNode<Control>(pauseScreenPacked, menusContainer)
                .GlobalPosition = GetViewport().GetCamera2D().GlobalPosition;
        }

        public void InitWin()
        {
            Timer lTimer = new Timer();
            lTimer.OneShot = true;
            lTimer.WaitTime = .5f;
            AddChild(lTimer);
            lTimer.Start();
            lTimer.Timeout += LoadWinScreen;

        }

        private void LoadWinScreen()
        {
            NodeCreator.CreateNode<Control>(GD.Load<PackedScene>(FilePath.PATH_WINSCREEN), menusContainer)
                .GlobalPosition = GetViewport().GetCamera2D().GlobalPosition;
        }

        protected override void Dispose(bool pDisposing)
        {
            if (Instance == this)
                Instance = null;
            base.Dispose(pDisposing);
        }
    }
}
