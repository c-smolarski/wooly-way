using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Scripts.Utils;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.UI.ClickableAreas
{
    public partial class LevelButton : ClickableArea
    {
        [Export] private Mountain mountain;
        [Export] private int levelNumber;
        [Export] private int displayFrame;
        [Export] private float radius = 500f;
        [Export] private float cameraTilt = 75f;

        private float EllipsisFactor => MathF.Cos(Mathf.DegToRad(cameraTilt));

        private float FrameAngle
        {
            get => angle + Mathf.Pi * 0.5f;
            set => angle = Mathf.DegToRad((value - displayFrame) * 360f / mountain.FrameCount);
        }

        private static readonly Vector2 levelScale = Vector2.One * 0.3f;

        private float angle;
        private Vector2 circleCenter;
        private MapInfo levelInfo;
        private Grid level;
        private ConvexPolygonShape2D shape;

        public override void _Ready()
        {
            base._Ready();
            levelInfo = LevelManager.MapData.Worlds["World1"]["Level6"];
            circleCenter = Position - new Vector2(0, radius * EllipsisFactor);
            
            mountain.LevelsVisibilityChanged += ChangeVisibilty;

            DisplayGrid();
            ChangeVisibilty(false);
        }

        private void InitMove()
        {
            mountain.FrameChanged += Move;
            Move(mountain.CurrentFrameIndex);
        }

        private void Move(int pFrame)
        {
            FrameAngle = pFrame;
            ZIndex = (int)(Position.Y - circleCenter.Y);
            Position = circleCenter + (radius * new Vector2(MathF.Cos(FrameAngle), MathF.Sin(FrameAngle) * EllipsisFactor));

            Scale = levelScale * new Vector2(MathF.Cos(FrameAngle + Mathf.Pi / 2), 1) //Scale adjusted to have 0 on the sides and 1 at center.
                * (MathF.Sin(FrameAngle) + 2); //Smallest value is 1, largest is 3.
        }

        private void DisplayGrid()
        {
            level = Grid.GenerateFromFile(levelInfo, this);
            level.Scale = levelScale;

            Vector2 lSize = level.PixelSize * 0.5f;
            shape = (ConvexPolygonShape2D)Collider.Shape;
            shape.Points = new Vector2[4]{
                new Vector2(lSize.X, 0),
                new Vector2(0, lSize.Y),
                new Vector2(-lSize.X, 0),
                new Vector2(0, -lSize.Y)
            };
        }

        private void ChangeVisibilty(bool pDisplayed)
        {
            Visible = pDisplayed;
            Collider.SetDeferred(nameof(Collider.Disabled), !pDisplayed);
            if (pDisplayed)
                InitMove();
            else
                mountain.FrameChanged -= Move;
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
    }
}
