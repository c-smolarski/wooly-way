using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.Managers
{
    public partial class SignalBus : Node
    {
        [Signal] public delegate void LevelClickedEventHandler(int pLevelNumber);
        [Signal] public delegate void StartLevelEventHandler(Grid pGrid);
        [Signal] public delegate void UnlockLevelEventHandler(int pLevelNumber);
        [Signal] public delegate void MoveToLevelButtonEventHandler(int pCurrentLevelNumber, bool pToNextLevel);
        [Signal] public delegate void ArrivedAtLevelButtonEventHandler(int pLevelNumber);
        [Signal] public delegate void UndoRedoParEventHandler(bool pIsUndo);
        [Signal] public delegate void LevelLoadedEventHandler();
        [Signal] public delegate void LevelRestartedEventHandler();
        [Signal] public delegate void LevelFinishedEventHandler();

        [Signal] public delegate void TutorialChangedVisibilityEventHandler(bool pVisible);
        [Signal] public delegate void TutorialFinishedEventHandler();

        [Signal] public delegate void AnyMenuButtonPressedEventHandler();
        [Signal] public delegate void PausingMenuDisplayedEventHandler();
        [Signal] public delegate void LanguageChangedEventHandler();
        [Signal] public delegate void FullscreenButtonPressedEventHandler();

        public static SignalBus Instance { get; private set; }

        public override void _Ready()
        {
            #region Singleton
            if (Instance != null)
            {
                GD.Print("Error : " + nameof(SignalBus) + " already exists. The new one is being freed...");
                QueueFree();
                return;
            }
            Instance = this;
            #endregion
        }

        protected override void Dispose(bool pDisposing)
        {
            if (Instance == this)
                Instance = null;
            base.Dispose(pDisposing);
        }
    }
}
