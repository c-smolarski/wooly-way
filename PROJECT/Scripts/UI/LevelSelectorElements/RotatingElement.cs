using Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables;
using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements
{
    public abstract partial class RotatingElement : LevelSelectorElement
    {
        [Export] public Mountain Mountain { get; private set; }
        [Export] public float Radius { get; private set; } = 500f;
        [Export] private float cameraTilt = 75f;

        //Frame at which object is displayed at center of screen.
        public int DisplayFrame { get; protected set; }

        //Pos of object when DisplayFrame is currently displayed.
        public Vector2 PosOnMountain => circleCenter + new Vector2(0, Radius * EllipsisFactor);

        //As camera is not directiy pointing down we have to apply it's cosine to circle's sine to get the resulting ellipsis.
        private float EllipsisFactor => MathF.Cos(Mathf.DegToRad(cameraTilt));

        //Gets angle from frame.
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
            circleCenter = new Vector2(0, Position.Y) - new Vector2(0, Radius * EllipsisFactor);

            Scale *= ScaleModifier;

            ScaleModifierChanged += () => Move(Mountain.CurrentFrameIndex);
        }

        //Updates Position before subscribing Move func.
        private void InitMove()
        {
            Move(Mountain.CurrentFrameIndex);
            Mountain.FrameChanged += Move;
        }

        private void Move(int pFrame)
        {
            FrameAngle = pFrame;
            Position = circleCenter + (Radius * new Vector2(MathF.Cos(FrameAngle), MathF.Sin(FrameAngle) * EllipsisFactor));
            Scale = levelScale * ScaleModifier * new Vector2(MathF.Cos(FrameAngle - Mathf.Pi / 2), 1) //Scale adjusted to have 0 on the sides and 1 at center.
                * (MathF.Sin(FrameAngle) + 2); //Smallest value is 1, largest is 3.
            ZIndex = (int)(Position.Y - circleCenter.Y);
        }

        // Stops moving when hidden for optimization.
        protected override void ChangeVisibilty(bool pDisplayed)
        {
            if (pDisplayed != alreadyDisplayed)
            {
                if (pDisplayed)
                    InitMove();
                else
                    Mountain.FrameChanged -= Move;
                base.ChangeVisibilty(pDisplayed);
            }
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
