using Com.IsartDigital.WoolyWay.GameObjects;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables;
using Com.IsartDigital.WoolyWay.UI.Menus;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements
{
    public partial class LevelSelectorPathFollow : PathFollow2D
    {
        [Export] private LevelButton level;
        [Export] private PathFollow2D cameraPath;
        [Export] private float speed = 1250f;

        private AnimatedSprite2D Player => level.LevelSelector.PlayerSprite;
        private bool IsAtStart => level.LevelNumber == LevelButton.START_LEVEL_NUMBER && ProgressRatio == 0;
        private Vector2I PlayerSpriteDirection => toNextLevel ? new Vector2I(0, -1) : new Vector2I(1, 0);
        public static Vector2 CurrentCameraGlobalPos { get;private set; }

        private Vector2 prevPos;
        private int prevSign;
        private bool isMoving, toNextLevel = true;

        public override void _Ready()
        {
            base._Ready();
            prevPos = Position;
            GetViewport().SizeChanged += OnWindowSizeChanged;
            SignalBus.Instance.FullscreenButtonPressed += OnWindowSizeChanged;
            SignalBus.Instance.MoveToLevelButton += InitMove;
        }

        private void OnWindowSizeChanged()
        {
            if ((LevelSelector.SelectedLevelNumber == level.LevelNumber && !LevelSelector.IsMoving) || (isMoving && LevelSelector.SelectedLevelNumber == level.LevelNumber + (toNextLevel ? -1 : 0)))
                GetViewport().GetCamera2D().GlobalPosition = cameraPath.GlobalPosition;
        }

        public override void _Process(double pDelta)
        {
            base._Process(pDelta);
            if (isMoving && CanMove())
                Move((float)pDelta);
            else if (isMoving)
            {
                isMoving = false;
                StringDirection.SetSpriteDirection(PlayerSpriteDirection, Player, StringDirection.IDLE);
                SignalBus.Instance.EmitSignal(SignalBus.SignalName.ArrivedAtLevelButton, level.LevelNumber + (toNextLevel ? 0 : -1));
            }
        }

        private void InitMove(int pLevelNumber, bool pToNext)
        {
            if (pLevelNumber == level.LevelNumber)
            {
                ProgressRatio = pToNext ? 0 : 1;
                toNextLevel = pToNext;
                isMoving = true;
                StringDirection.SetSpriteDirection(PlayerSpriteDirection, Player, StringDirection.WALK);
            }
        }

        private void Move(float pDelta)
        {
            Progress += (float)pDelta * speed * (toNextLevel ? 1 : -1);

            int lCurrentSign = ProgressRatio == (int)ProgressRatio ? Mathf.Sign(Player.Scale.X) : Mathf.Sign(Position.X - prevPos.X);
            if(lCurrentSign != 0)
            {
                if (lCurrentSign != prevSign)
                    Player.Scale = new Vector2(Player.Scale.Abs().X * lCurrentSign, Player.Scale.Y);
                prevSign = lCurrentSign;
            }
            Player.GlobalPosition = GlobalPosition;
            prevPos = Position;
            SoundManager.GetInstance().PlaySFXFromArray(SoundManager.GetInstance().FootstepsSFX, Mobile.MOVE_DURATION);

            cameraPath.ProgressRatio = ProgressRatio;
            CurrentCameraGlobalPos = GetViewport().GetCamera2D().GlobalPosition = cameraPath.GlobalPosition;
        }

        private bool CanMove()
        {
            return (toNextLevel && ProgressRatio < 1) || (!toNextLevel && ProgressRatio > 0);
        }

        protected override void Dispose(bool pDisposing)
        {
            SignalBus.Instance.MoveToLevelButton -= InitMove;
            SignalBus.Instance.FullscreenButtonPressed -= OnWindowSizeChanged;
            if (IsInstanceValid(GetViewport()))
                GetViewport().SizeChanged -= OnWindowSizeChanged;
            base.Dispose(pDisposing);
        }
    }
}
