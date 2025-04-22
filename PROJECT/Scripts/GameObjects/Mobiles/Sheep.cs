using Com.IsartDigital.WoolyWay.Components;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;
using System.Collections.Generic;
using Com.IsartDigital.WoolyWay.Assets;

// Author : Camille SMOLARSKI & Tom BAGNARA & Alissa DELATTRE

namespace Com.IsartDigital.WoolyWay.GameObjects.Mobiles
{
    public partial class Sheep : Mobile
    {
        [Export] private ClickableArea clickable;
        [Export] private Node2D exclamationPoint;
        [Export] private GpuParticles2D spark;

        private const float SHEEP_REACTION_TIME = 0.1f;

        private const float EXCLAMATION_APPEAR_DURATION = 0.15f;
        private const float EXCLAMATION_DISAPPEAR_DURATION = 0.5f;

        private const float BLEAT_ANIM_DURATION = 1f;
        private const float BLEAT_ANIM_STRENGTH = 1.1f;
        private const int BLEAT_ANIM_SHAKE_NUMBER = 15;
        private const int BLEAT_RANDOM_LIST_SIZE = 15;

        private static int[] BleatRandomList => new int[BLEAT_RANDOM_LIST_SIZE] { 1, 2, 2, 2, 3, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5 };

        public bool IsWin => CurrentTile.IsFlag;
        public bool IsRotating { get; private set; }

        private int movementsBeforeBleat;
        public bool hasMovedASecondTime, isUseful;
        
        private List<Vector2I> directions = new List<Vector2I>(){
            Vector2I.Down, Vector2I.Left, Vector2I.Up, Vector2I.Right,
        };

        [Export]private Color[] sheepColor = new Color[] {
            new Color(0.792f, 1f, 1f, 1f),
            new Color(1f, 0.898f, 1f, 1f),
            new Color(0.851f, 0.859f, 1f, 1f),
            new Color(0.848f, 0.956f, 0.88f, 1f),
            new Color(0.966f, 0.936f, 0.795f, 1f)
        };

        private Vector2 baseScale;

        private Tween tween;
        //private Tween tweenColor;

        private Vector2 tweenValues = new Vector2();
        private bool breathOut = false;
        private float breathTarget = .47f;

        private RandomNumberGenerator rand = new RandomNumberGenerator();

        public override void _Ready()
        {
            base._Ready();
            rand.Randomize();
            baseScale = Renderer.Scale;
            tweenValues = new Vector2(breathTarget, Renderer.Scale.X);

            Timer timer = new Timer();
            timer.WaitTime = rand.RandfRange(0f, 1.5f);
            timer.OneShot = true;
            AddChild(timer);
            timer.Start();
            timer.Timeout += () => SheepBreath(tweenValues.X);

            clickable.Clicked += Clicked;
            SetBleatTimer();
            StringDirection.SetSpriteDirection(Vector2I.Down, Renderer);
            
        }
        private void SheepBreath(float pTarget)
        {
            tween = CreateTween();
            tween.TweenProperty(Renderer, TweenProp.SCALE, new Vector2(Renderer.Scale.X, pTarget), 1.5f);
            tween.Finished += NextBreath;


        }

        private void ColorChanging()
        {
            int index = rand.RandiRange(0, sheepColor.Length - 1);
            Tween tweenColor = CreateTween();
            for (int i = index; i < sheepColor.Length; i++)
            {
                tweenColor.TweenProperty(Renderer, TweenProp.MODULATE, sheepColor[i], 3f);
            }
            tweenColor.Finished += ColorChanging;
        }

        private void NextBreath()
        {
            breathOut = !breathOut;
            if (breathOut)
            {
                SheepBreath(tweenValues.Y);
                return;
            }
            SheepBreath(tweenValues.X);
        }

        private void ColliderInit(Tile pTile)
        {
            ((ConvexPolygonShape2D)clickable.Collider.Shape).Points = pTile.ColliderShapePoints;
            clickable.Collider.GlobalScale = pTile.GlobalScale;
        }

        /// <summary>
        /// Checks a player is next the sheep that was clicked, if yes the sheep moves and the player takes it place
        /// </summary>
        private void Clicked()
        {
            List<Tile> lNeighbors = Grid.Neighbors(CurrentTile);
            int lNumNeighbors = lNeighbors.Count;
            for (int i = 0; i < lNumNeighbors; i++)
            {
                if (lNeighbors.Contains(Grid.ObjectDict[Grid.Player]))
                {
                    Vector2I lDirection =  CurrentTile.Index - Grid.ObjectDict[Grid.Player].Index;
                    Grid.Player.InitMove(lDirection);
                    return;
                }
            }
        }

        /// <summary>
        /// Checks if the current Sheep can move onto the next Tile depending on if it's its first or second step and the content of the lNextTile.
        /// </summary>
        /// <param name="pMoveDirection"></param>
        /// <returns>true if the Sheep can be moved onto the next tile. Otherwise, false.</returns>
        public bool CanMove(Vector2I pMoveDirection)
        {
            Tile lNextTile = Grid.IndexDict[CurrentTile.Index + pMoveDirection];

            if (lNextTile.IsEmpty()) return true;
            
            if (lNextTile.Object is Obstacle || lNextTile.Object is Player) return false;
            
            return lNextTile.Object is not Sheep;

        }

        public override void InitMove(Vector2I pMoveDirection)
        {
            hasMovedASecondTime = false;
            FinishedMoving += MoveForward;
            base.InitMove(pMoveDirection);
            if (--movementsBeforeBleat == 0)
                Bleat();
        }

        protected override void MoveTo(Tile pTile)
        {
            base.MoveTo(pTile);
            if (CurrentTile?.Dog != null)
            {
                FinishedMoving -= MoveForward;
                FinishedMoving += OnWalkedToDog;
            }
        }

        private void MoveForward()
        {
            FinishedMoving -= MoveForward;
            hasMovedASecondTime = true;
            base.InitMove(Direction);

            if (IsMoving)
            {
                FinishedMoving += WinCheck;
                if (CurrentTile.Dog == null && !CurrentTile.IsFlag)
                    FinishedMoving += SetPlayerCanMove;

                if (--movementsBeforeBleat == 0)
                    Bleat();
            }
            else
                WinCheck();
        }

        private void WinCheck()
        {
            FinishedMoving -= WinCheck;

            if (isUseful && IsWin)
                CurrentTile.butterfly.Emitting = true;

            if (IsWin && Grid.WinCheck())
            {
                CurrentTile.butterfly.Finished += OnButterflyEnd;
                SignalBus.Instance.EmitSignal(SignalBus.SignalName.LevelFinished);
            }
            else if (!IsMoving)
                SetPlayerCanMove();

        }

        private void OnButterflyEnd()
        {
            CurrentTile.butterfly.Finished -= OnButterflyEnd;
            SetPlayerCanMove();
            Grid.InitWin();
        }

        private async void OnWalkedToDog()
        {
            FinishedMoving -= OnWalkedToDog;
            IsRotating = true;
            CurrentTile?.Dog?.Bark();
            await ToSignal(GetTree().CreateTimer(SHEEP_REACTION_TIME, false), Timer.SignalName.Timeout);
            RotateAnim();
        }

        private void RotateAnim()
        {
            Tween lTween = CreateTween();
            lTween.TweenProperty(exclamationPoint, TweenProp.MODULATE_ALPHA, 1, 0f);
            lTween.Parallel().TweenProperty(exclamationPoint, TweenProp.SCALE_X, exclamationPoint.Scale.X, EXCLAMATION_APPEAR_DURATION)
                .From(0f);
            lTween.Parallel().TweenProperty(exclamationPoint, TweenProp.SCALE_Y, exclamationPoint.Scale.Y, EXCLAMATION_APPEAR_DURATION)
                .From(0f)
                .SetTrans(Tween.TransitionType.Elastic)
                .SetEase(Tween.EaseType.Out);
            lTween.TweenInterval(SHEEP_REACTION_TIME);
            lTween.Finished += () =>
            {
                Rotate();
                Tween lTween2 = CreateTween();
                lTween2.TweenProperty(exclamationPoint, TweenProp.MODULATE_ALPHA, 0, EXCLAMATION_DISAPPEAR_DURATION);
            };

        }

        /// <summary>
        /// Changes the Direction of the Sheep. Should only be called if Sheep is on a Dog Tile.
        /// </summary>
        private void Rotate()
        {
            int lRotationIndex = directions.IndexOf(Direction);
            if (++lRotationIndex >= directions.Count)
                lRotationIndex = 0;

            Direction = directions[lRotationIndex];
            StringDirection.SetSpriteDirection(Direction, Renderer);
            IsRotating = false;

            if (!hasMovedASecondTime)
                MoveForward();
            else
                SetPlayerCanMove();
        }

        private void Bleat()
        {
            SetBleatTimer();
            if(SoundManager.GetInstance().PlaySFXFromArray(SoundManager.GetInstance().SheepSFXList, 0.5f))
            {
                Tween lTween = CreateTween();
                for (int i = 0; i < BLEAT_ANIM_SHAKE_NUMBER; i++)
                    lTween.TweenProperty(this, TweenProp.SCALE, Scale * BLEAT_ANIM_STRENGTH, BLEAT_ANIM_DURATION / BLEAT_ANIM_SHAKE_NUMBER)
                        .From(Scale);
                lTween.TweenProperty(this, TweenProp.SCALE, Scale, 0f);
            }
        }

        private void SetBleatTimer()
        {
            movementsBeforeBleat = BleatRandomList[new Random().Next(BLEAT_RANDOM_LIST_SIZE)];
        }

        private void SetPlayerCanMove()
        {
            FinishedMoving -= SetPlayerCanMove;
            Player.canStartMoving = !IsAnySheepMoving(Grid);
        }

        public static bool IsAnySheepMoving(Grid pGrid)
        {
            bool lMoving = false;
            foreach (GameObject lObj in pGrid.ObjectDict.Values)
                if (lObj is Sheep && (((Sheep)lObj).IsMoving || ((Sheep)lObj).IsRotating))
                    lMoving = true;
            return lMoving;
        }

        public static Sheep Create(PackedScene pScene, Tile pTile, Vector2I pDirection, bool pUseful = true)
        {
            Sheep lSheep = Create<Sheep>(pScene, pTile);
            lSheep.Direction = pDirection.Sign();
            lSheep.isUseful = pUseful;
            if (lSheep.isUseful)
            {
                lSheep.ColorChanging();
                lSheep.spark.Visible = true;
            }
            //if(!lSheep.isUseful) 
            //    lSheep.Modulate = Colors.DarkGray;
            lSheep.ColliderInit(pTile);
            return lSheep;
        }

        protected override void Dispose(bool disposing)
        {
            clickable.Clicked -= Clicked;
            FinishedMoving -= MoveForward;
            FinishedMoving -= WinCheck;
            FinishedMoving -= OnWalkedToDog;
            base.Dispose(disposing);
        }
    }
}
