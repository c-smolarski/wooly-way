using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.UI.Buttons.ButtonsWithHoverAnim.MainButtons;
using Godot;
using System;

// Author : Alissa Delattre

namespace Com.IsartDigital.WoolyWay
{
    public partial class Tutorial : Sprite2D
    {

        private const int NUM_TUTORIAL = 3;
        private const string TUTORIAL_NAME = "Tutorial";
        private const float TUTORIAL_SIZE = .8f;
        private const int ZINDEX_LEVEL = 100;

        [Export] private Label[] tutoText;
        [Export] public Marker2D marker;

        public static Tutorial Instance { get; private set; }

        public override void _Ready()
        {
            #region Singleton
            if (Instance != null)
            {
                GD.Print("Error : " + nameof(Tutorial) + " already exists. The new one is being freed...");
                QueueFree();
                return;
            }
            Instance = this;
            #endregion
            ZIndex = ZINDEX_LEVEL;
        }

        /// <summary>
        /// Opens the next tutorial at the end
        /// </summary>
        public void NexTutorial()
        {
            tutoText[CloseTutorial.index-1].Visible = false;
            tutoText[CloseTutorial.index].Visible = true;

            CloseTutorial.index += 1;
            LevelManager.CurrentLevel.QueueFree();

            if (CloseTutorial.index > NUM_TUTORIAL)
            {
                CloseTutorial.CloseUndoRedo();
                GD.Print("finish");
                SignalBus.Instance.EmitSignal(SignalBus.SignalName.TutorialFinished);
                return;
            }

            LevelManager.GetInstance().GenerateLevel(
                LevelManager.MapData.Worlds[TUTORIAL_NAME][CloseTutorial.index.ToString()],
                GameManager.Instance.TutorialContainer
                );
            LevelManager.CurrentLevel.Scale *= TUTORIAL_SIZE;
        }

        protected override void Dispose(bool pDisposing)
        {
            if (Instance == this)
                Instance = null;
            base.Dispose(pDisposing);
        }
    }
}
