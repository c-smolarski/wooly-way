using Godot;
using System;

namespace Com.IsartDigital.WoolyWay.UI.ClickableAreas
{
    public partial class Mountain : ClickableArea
    {
        [Signal] public delegate void FrameChangedEventHandler(int pFrame);
        [Signal] public delegate void LevelsVisibilityChangedEventHandler(bool pDisplayed);

        private const string RENDERER_PATH = "renderer";

        public int FrameCount => renderer.SpriteFrames.GetFrameCount(renderer.Animation);
        public int CurrentFrameIndex => renderer.Frame;

        private AnimatedSprite2D renderer;

        public override void _Ready()
        {
            base._Ready();
            renderer = GetNode<AnimatedSprite2D>(RENDERER_PATH);
            renderer.FrameChanged += () => EmitSignal(SignalName.FrameChanged, CurrentFrameIndex);
            EmitSignal(SignalName.LevelsVisibilityChanged, true);
            renderer.Play();
        }
    }
}