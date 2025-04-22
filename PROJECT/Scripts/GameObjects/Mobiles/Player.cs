using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;
using System.Collections.Generic;

// Author : Camille SMOLARSKI && Alissa DELATTRE

namespace Com.IsartDigital.WoolyWay.GameObjects.Mobiles
{
    public partial class Player : Mobile
    {
        public enum ControlScheme
        {
            ALTERNATIVE = -90,
            CLASSIC = 0
        }

        public static bool CurrentlyMoving { get; private set; }

        //Movement is laggy in FTUE if the backing field is the static property?
        protected override bool IsMoving
        {
            get => backingIsMoving;
            set => CurrentlyMoving = backingIsMoving = value;
        }
        public static bool canStartMoving = true;

        public List<Vector2I> path = new List<Vector2I>();
        public int steps = 0;
        private bool backingIsMoving;
        private bool animationSignalConected = false;

        public override void _Ready()
        {
            canStartMoving = true;
            FinishedMoving += MoveStepByStep;
            InputManager.Instance.MoveInputPressed += OnMoveInput;
            SignalBus.Instance.LevelLoaded += SetCanMove;
            SignalBus.Instance.UndoRedoPar += (bool pIsUndo) => StepUndoRedo(pIsUndo);
            SignalBus.Instance.LevelRestarted += ResetPar;
            Renderer.Play();
        }

        private void OnMoveInput(Vector2I pMoveDirection)
        {
            InitMove(pMoveDirection.Rotate((int)GameManager.currentControlScheme));
        }

        public override void InitMove(Vector2I pMoveDirection)
        {
            if (Grid != LevelManager.CurrentLevel || !canStartMoving || IsMoving) return;

            if (animationSignalConected)
            {
                Renderer.AnimationFinished -= RendererOnAnimationFinished;
                animationSignalConected = false;
            }

            base.InitMove(pMoveDirection);
            Direction = pMoveDirection;
            if (IsMoving)
            {
                Grid.Player.Step();
                canStartMoving = !Sheep.IsAnySheepMoving(Grid);
            }
            Renderer.AnimationFinished += RendererOnAnimationFinished;
            animationSignalConected = true;
        }



        /// <summary>
        ///Moves the player tile by tile until he reaches the target
        /// </summary>
        public void MoveStepByStep()
        {
            if (path.Count == 0 || IsMoving) return;
            int lIndex = path.Count-1;
            InitMove(path[lIndex] - CurrentTile.Index);
            path.RemoveAt(lIndex);
        }

        private void SetCanMove()
        {
            canStartMoving = true;
            IsMoving = false;
        }

        private void Step()
        {
            if (LevelManager.isTutorial) return;
            steps++;
            HudManager.Instance.ActualizeHud(steps);
        }

        private void ResetPar()
        {
            if (Grid != LevelManager.CurrentLevel) return;
            steps = 0;
            HudManager.Instance.ActualizeHud(steps);
        }

        private void StepUndoRedo(bool isUndo)
        {
            if (Grid != LevelManager.CurrentLevel) return;
            if (isUndo) steps--;
            else steps++;
            steps = Mathf.Max(steps, 0);
            HudManager.Instance.ActualizeHud(steps);
        }
        
        private void RendererOnAnimationFinished()
        {
            StringDirection.SetSpriteDirection(Direction, Renderer);
            Renderer.AnimationFinished -= RendererOnAnimationFinished;
            animationSignalConected = false;
        }

        protected override void Dispose(bool disposing)
        {
            InputManager.Instance.MoveInputPressed -= OnMoveInput;
            SignalBus.Instance.LevelLoaded -= SetCanMove;
            SignalBus.Instance.LevelRestarted -= ResetPar;
            FinishedMoving -= MoveStepByStep;
            SetCanMove();
            base.Dispose(disposing);
        }
    }
}
