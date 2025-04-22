using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.UI.LevelSelectorElements;
using Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.UI.Menus
{
    public partial class LevelSelector : Node2D
    {
        [Export(PropertyHint.Range, "1, 100, 1")] public int WorldNumber { get; private set; } = 1;
        [Export] public AnimatedSprite2D PlayerSprite { get; private set; }
        [Export] private Node2D levelsContainer;

        public const int DEFAULT_WORLD_NUMBER = 1;
        public const float TRANSITION_TIME = 0.5f;

        public static bool IsMoving { get; private set; }
        public static int UnlockedLevels { get; private set; }
        public static int SelectedLevelNumber => selectedLevel?.LevelNumber ?? LevelButton.START_LEVEL_NUMBER;

        public static LevelButton selectedLevel;

        private bool isTransitioning, onEndPos, onStartPos = true;

        public override void _Ready()
        {
            base._Ready();
            GetViewport().GetCamera2D().GlobalPosition = GlobalPosition;
            GetViewport().SizeChanged += OnWindowSizeChange;
            InputManager.Instance.MoveInputPressed += ChangeLevel;
            SignalBus.Instance.MoveToLevelButton += OnStartMoving;
            SignalBus.Instance.ArrivedAtLevelButton += OnArriveAtLevel;
            SignalBus.Instance.StartLevel += OnLevelStarted;
            SignalBus.Instance.UnlockLevel += OnLevelUnlock;
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.UnlockLevel, UnlockedLevels = GetUnlockedLevelsNumber());
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.ArrivedAtLevelButton, LevelButton.START_LEVEL_NUMBER);
            EnvironmentManager.GetInstance().ChangeWeather(EnvironmentManager.Weather.DAY);
        }

        private void OnLevelUnlock(int pLevelNumber)
        {
            UnlockedLevels = GetUnlockedLevelsNumber();
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.ArrivedAtLevelButton, SelectedLevelNumber);
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);
            if (!onStartPos && !isTransitioning && Input.IsActionJustPressed(InputManager.UI_ACCEPT))
                SignalBus.Instance.EmitSignal(SignalBus.SignalName.LevelClicked, selectedLevel.LevelNumber);
        }

        private void OnWindowSizeChange()
        {
            GetViewport().GetCamera2D().GlobalPosition = LevelSelectorPathFollow.CurrentCameraGlobalPos;
        }

        private void ChangeLevel(Vector2I pDirection)
        {
            bool lToNextLevel = pDirection.Y < 0;
            if (pDirection.Y != 0 && CanMove(lToNextLevel))
            {
                SignalBus.Instance.EmitSignal(SignalBus.SignalName.MoveToLevelButton, SelectedLevelNumber + (lToNextLevel ? 1 : 0), lToNextLevel);
                SoundManager.GetInstance().PlaySFXFromArray(SoundManager.GetInstance().UIButtonClick);
            }
        }

        private void OnStartMoving(int pCurrentLevelNumber, bool pToNextLevel)
        {
            IsMoving = true;
            foreach (LevelButton lLevel in levelsContainer.GetChildren())
                if (lLevel.LevelNumber == pCurrentLevelNumber + (pToNextLevel ? 0 : -1))
                    EnvironmentManager.GetInstance().ChangeWeather(lLevel.Weather);
        }

        private void OnArriveAtLevel(int pLevelNumber)
        {
            IsMoving = false;
            onEndPos = pLevelNumber == levelsContainer.GetChildCount();
            if (onStartPos = pLevelNumber == LevelButton.START_LEVEL_NUMBER)
                selectedLevel = null;
        }

        private void OnLevelStarted(Grid pGrid)
        {
            isTransitioning = true;
            InputManager.Instance.MoveInputPressed -= ChangeLevel;
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, TweenProp.MODULATE_ALPHA, 0, TRANSITION_TIME);
            lTween.Finished += QueueFree;

        }

        private int GetUnlockedLevelsNumber()
        {
            for (int i = DataManager.UserUnlockedLevels.Count - 1; i > 0; i--)
                if (DataManager.UserUnlockedLevels[i])
                    return i + 1;
            //GameManager.Instance.buttonTuto.OnPressed();
            GameManager.Instance.Opentuto();
            return LevelButton.START_LEVEL_NUMBER + 1;
        }

        private bool CanMove(bool pToNextLevel)
        {
            return !IsMoving && (pToNextLevel || !onStartPos) && (!pToNextLevel || !onEndPos) && SelectedLevelNumber + (pToNextLevel ? 1 : 0) <= UnlockedLevels && LevelManager.CurrentLevel == null;
        }

        protected override void Dispose(bool disposing)
        {
            if(IsInstanceValid(GetViewport()))
                GetViewport().SizeChanged -= OnWindowSizeChange;
            IsMoving = false;
            InputManager.Instance.MoveInputPressed -= ChangeLevel;
            SignalBus.Instance.MoveToLevelButton -= OnStartMoving;
            SignalBus.Instance.ArrivedAtLevelButton -= OnArriveAtLevel;
            SignalBus.Instance.StartLevel -= OnLevelStarted;
            SignalBus.Instance.UnlockLevel -= OnLevelUnlock;
            base.Dispose(disposing);
        }
    }
}
