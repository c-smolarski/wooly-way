using Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables;
using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements
{
    public abstract partial class RotatingElement : LevelSelectorElement
    {
        [Export] public Mountain Mountain { get; private set; }
        [Export] public int DisplayFrame { get; private set; }
        [Export] private float radius = 500f;
        [Export] private float cameraTilt = 75f;

        public Vector2 PosOnMountain => circleCenter + new Vector2(0, radius * EllipsisFactor);
        private float EllipsisFactor => MathF.Cos(Mathf.DegToRad(cameraTilt));
        private float FrameAngle
        {
            get => angle + Mathf.Pi * 0.5f;
            set => angle = Mathf.DegToRad((value - DisplayFrame) * 360f / Mountain.FrameCount);
        }

        protected static readonly Vector2 levelScale = Vector2.One * 0.3f;

        private float angle;
        private bool alreadyDisplayed;
        private Vector2 circleCenter;

        public override void _Ready()
        {
            base._Ready();
            circleCenter = Position - new Vector2(0, radius * EllipsisFactor);

            Scale *= ScaleModifier;

            ScaleModifierChanged += () => Move(Mountain.CurrentFrameIndex);
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

        protected override void Dispose(bool disposing)
        {
            Mountain.FrameChanged -= Move;
            ScaleModifierChanged -= () => Move(Mountain.CurrentFrameIndex);
            base.Dispose(disposing);
        }
    }
}
