using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.UI.Menus;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim.MainButtons
{
    public partial class CloseTutorial : MainButton
    {
        //Exports
        [Export] private PackedScene undoRedoPacked;
        [Export] private PackedScene framePacked;

        private static History undoRedo;
        public static Grid oldGrid;
        private static CloseTutorial oldButton;
        private Tutorial frame;
        private Node2D gameContainer;

        private static bool opened = false;
        public static int index = 0;
        //private Vector2 positionTutorial = new Vector2(450, 300f);
        private Vector2 positionHistoryFromTutorial = new Vector2(0, -177);

        private const string TUTORIAL_NAME = "Tutorial";
        private const float TUTORIAL_SIZE = .8f;
        private const int MAX_INDEX = 4;

        public override void _Ready()
        {
            base._Ready();
            SignalBus.Instance.LevelLoaded += Enable;
            SignalBus.Instance.LevelFinished += Disable;
            SignalBus.Instance.TutorialFinished += Enable;
        }

        private void Enable()
        {
            Disabled = false;
        }

        protected override void OnPressed()
        {
            if (LevelSelector.IsMoving)
                return;

            if (opened)
                CloseTuto();
            else if (!Player.CurrentlyMoving)
                OpenTuto();

            base.OnPressed();
        }

        public void OpenTuto()
        {
            Visible = false;
            oldButton = this;
            opened = true;
            index += 1;

            GridManagement();
            FrameInstanciation();

            SignalBus.Instance.EmitSignal(SignalBus.SignalName.TutorialChangedVisibility, true);

            UndoRedoInstanciation();
        }

        /// <summary>
        /// Creates the new grid and stock the old one is a variable to reactivate it at the end
        /// </summary>
        private void GridManagement()
        {
            LevelManager.isTutorial = true;
            oldGrid = LevelManager.CurrentLevel;
            LevelManager.GetInstance().GenerateLevel(
                LevelManager.MapData.Worlds[TUTORIAL_NAME][index.ToString()],
                GameManager.Instance.TutorialContainer
                );
            LevelManager.CurrentLevel.Scale *= TUTORIAL_SIZE;
        }

        /// <summary>
        /// Instantiate the undo redo
        /// </summary>
        private void UndoRedoInstanciation()
        {
            undoRedo = NodeCreator.CreateNode<History>(
                undoRedoPacked,
                GameManager.Instance.TutorialContainer
                );
            undoRedo.ZIndex = GameManager.Instance.TutorialContainer.ZIndex;
            undoRedo.GlobalPosition = new Vector2(Tutorial.Instance.marker.GlobalPosition.X, Tutorial.Instance.marker.GlobalPosition.Y) + positionHistoryFromTutorial;
            undoRedo.Scale = new Vector2(0.75f, 0.75f);
        }

        /// <summary>
        /// Instantiate the frame
        /// </summary>
        private void FrameInstanciation()
        {
            frame = NodeCreator.CreateNode<Tutorial>(
                framePacked,
                GameManager.Instance.TutorialContainer
                );
            frame.ZIndex--;
            frame.GlobalPosition = GameManager.Instance.camera.GlobalPosition;
        }

        public static void CloseUndoRedo()
        {
            undoRedo.QueueFree();
        }

        /// <summary>
        /// Close the tutorial and reactivates the old grid
        /// </summary>
        private void CloseTuto()
        {
            LevelManager.isTutorial = false;
            oldButton.Visible = true;
            opened = false;
            if (index < MAX_INDEX)
            {
                LevelManager.CurrentLevel.QueueFree();
                CloseUndoRedo();
            }
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.TutorialChangedVisibility, false);

            index = 0;
            LevelManager.CurrentLevel = oldGrid;
            GetParent()?.GetParent()?.QueueFree();
            Visible = true;

        }

        protected override void Dispose(bool pDisposing)
        {
            SignalBus.Instance.TutorialFinished -= Enable;
            SignalBus.Instance.LevelLoaded -= Enable;
            SignalBus.Instance.LevelFinished -= Disable;
            if (opened)
                CloseTuto();
            base.Dispose(pDisposing);
        }
    }
}
