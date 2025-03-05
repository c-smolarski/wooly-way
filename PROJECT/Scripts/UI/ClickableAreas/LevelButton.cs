using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Scripts.Utils;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.UI.ClickableAreas
{
    public partial class LevelButton : ClickableArea
    {
        [Export] public Mountain Mountain { get; private set; }
        [Export(PropertyHint.Range, "1,1000")] public int LevelNumber { get; private set; }
        [Export] public int DisplayFrame { get; private set; }
        [Export] private float radius = 500f;
        [Export] private float cameraTilt = 75f;
        [ExportGroup("On focus")]
        [Export] private float sinusoidFreq = 2.5f;
        [Export] private float sinusoidIntensity = 0.45f;

        public const int DISPLAY_NO_LEVEL = -1;
        private const string RIGHT_ARROW_PATH = "rightArrow";
        private const string LEFT_ARROW_PATH = "leftArrow";

        public Vector2 PosOnMountain => circleCenter + new Vector2(0, radius * EllipsisFactor);
        private float EllipsisFactor => MathF.Cos(Mathf.DegToRad(cameraTilt));

        private float FrameAngle
        {
            get => angle + Mathf.Pi * 0.5f;
            set => angle = Mathf.DegToRad((value - DisplayFrame) * 360f / Mountain.FrameCount);
        }

        private static readonly Vector2 levelScale = Vector2.One * 0.3f;

        private float angle, elapsedTime;
        private bool focused, alreadyDisplayed;
        private Vector2 circleCenter;
        private MapInfo levelInfo;
        private Grid grid;
        private ConvexPolygonShape2D shape;
        private LevelSelectorArrow leftArrow, rightArrow;

        public override void _Ready()
        {
            base._Ready();

            levelInfo = LevelManager.GetLevel(Mountain.WorldNumber, LevelNumber);

            circleCenter = Position - new Vector2(0, radius * EllipsisFactor);
            Scale *= ScaleModifier;
            
            Mountain.LevelsVisibilityChanged += DisplayLevel;
            ScaleModifierChanged += () => Move(Mountain.CurrentFrameIndex);

            DisplayGrid();
            ArrowsInit();
        }

        public override void _Process(double pDelta)
        {
            base._Process(pDelta);
            float lDelta = (float)pDelta;
            if (focused)
                FocusedAnim(lDelta);
        }

        private void ArrowsInit()
        {
            leftArrow = GetNodeOrNull<LevelSelectorArrow>(LEFT_ARROW_PATH);
            rightArrow = GetNodeOrNull<LevelSelectorArrow>(RIGHT_ARROW_PATH);

            foreach (LevelSelectorArrow lArrow in new LevelSelectorArrow[] { leftArrow, rightArrow })
            {
                if (!LevelManager.LevelExists(Mountain.WorldNumber, LevelNumber + (lArrow == rightArrow ? 1 : -1)))
                    lArrow?.QueueFree();
                if (lArrow == null || lArrow.IsQueuedForDeletion())
                    continue;
                lArrow.Position = MathS.PolarToCartesian(
                    grid.PixelSize.Length() * 0.5f, 
                    lArrow.Rotation + (lArrow.Scale.Sign().X >= 0 ? 0 : MathF.PI)
                );
            }
        }

        private void DisplayGrid()
        {
            grid = Grid.GenerateFromFile(levelInfo, this);
            grid.Scale = levelScale;

            Vector2 lSize = grid.PixelSize * 0.5f;
            shape = (ConvexPolygonShape2D)Collider.Shape;
            shape.Points = new Vector2[4]{
                new Vector2(lSize.X, 0),
                new Vector2(0, lSize.Y),
                new Vector2(-lSize.X, 0),
                new Vector2(0, -lSize.Y)
            };
        }

        private void DisplayLevel(int pLevelToDisplay)
        {
            ChangeVisibilty(IsNeighbour(pLevelToDisplay));

            if (LevelNumber == pLevelToDisplay)
            {
                Mountain.RotateToButton(this);
                focused = true;
            }
            else
            {
                focused = false;
                StopFocusedAnim();
            }
        }

        private void FocusedAnim(float pDelta)
        {
            elapsedTime += pDelta;
            float lSinusoid, lMaxValue = MathF.Max(grid.Size.X, grid.Size.Y) * 2f - 1;
            for (int i = 0; i < lMaxValue; i++)
            {
                lSinusoid = elapsedTime + (i / lMaxValue);
                foreach (Tile lTile in grid.GetTilesDiagonally(DiagonalDirection.TOP_RIGHT, i))
                {
                    lTile.Position += Vector2.Down * Mathf.Sin(lSinusoid * sinusoidFreq) * sinusoidIntensity;
                    if (grid.ObjectDict.Contains(lTile))
                        grid.ObjectDict[lTile].Position = lTile.Position;
                }
            }
        }

        private void StopFocusedAnim()
        {
            elapsedTime = default;
            grid.ResetTilesPos();
        }

        private void InitMove()
        {
            Move(Mountain.CurrentFrameIndex);
            Mountain.FrameChanged += Move;
        }

        private void Move(int pFrame)
        {
            FrameAngle = pFrame;
            Position = circleCenter + (radius * new Vector2(MathF.Cos(FrameAngle), MathF.Sin(FrameAngle) * EllipsisFactor));
            Scale = levelScale * ScaleModifier * new Vector2(MathF.Cos(FrameAngle - Mathf.Pi / 2), 1) //Scale adjusted to have 0 on the sides and 1 at center.
                * (MathF.Sin(FrameAngle) + 2); //Smallest value is 1, largest is 3.
            ZIndex = (int)(Position.Y - circleCenter.Y);
        }

        protected override void ChangeVisibilty(bool pDisplayed)
        {
            if (pDisplayed)
                InitMove();
            else
                Mountain.FrameChanged -= Move;

            if (pDisplayed != alreadyDisplayed)
                base.ChangeVisibilty(pDisplayed);

            alreadyDisplayed = pDisplayed;
        }

        private bool IsNeighbour(int pFromLevel)
        {
            return LevelNumber >= pFromLevel - 1 && LevelNumber <= pFromLevel + 1;
        }

        public static LevelButton Create(MapInfo pLevel)
        {
            LevelButton lBtn = NodeCreator.CreateNode<LevelButton>(
                GameManager.Instance.LevelButtonScene,
                GameManager.Instance.GameContainer
            );
            lBtn.levelInfo = pLevel;
            return lBtn;
        }

        protected override void Dispose(bool disposing)
        {
            Mountain.FrameChanged -= Move;
            ScaleModifierChanged -= () => Move(Mountain.CurrentFrameIndex);
            base.Dispose(disposing);
        }
    }
}
