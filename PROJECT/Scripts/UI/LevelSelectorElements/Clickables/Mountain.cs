using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.RotatingElements;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;

// Author : Camille Smolarski

namespace Com.IsartDigital.WoolyWay.UI.LevelSelectorElements.Clickables
{
    public partial class Mountain : ClickableLevelSelectorElement
    {
        [Signal] public delegate void FrameChangedEventHandler(int pFrame);

        [Export(PropertyHint.Range, "1,1000")] public int WorldNumber { get; private set; }
        [ExportGroup("On Focus")]
        [Export] private int defaultDisplayedLevel = 1;
        [Export] public Vector2 focusScale = Vector2.One * 2f;

        private const string RENDERER_PATH = "renderer";

        public int CurrentFrameIndex 
        {
            get => renderer.Frame;
            set {
                if (value < 0)
                    CurrentFrameIndex = value + FrameCount;
                else if (value > FrameCount)
                    CurrentFrameIndex = value - FrameCount;
                else
                    renderer.Frame = value;
            }
        }

        private int AnimFrameIndex
        {
            get => backingAnimIndex;
            set {
                CurrentFrameIndex = backingAnimIndex = value;
            }
        }

        public int FrameCount => renderer.SpriteFrames.GetFrameCount(renderer.Animation);
        public bool IsFocused { get; private set; }

        private AnimatedSprite2D renderer;
        private Vector2 defaultPos, defaulScale;
        private int backingAnimIndex;

        public override void _Ready()
        {
            base._Ready();
            renderer = GetNode<AnimatedSprite2D>(RENDERER_PATH);
            renderer.FrameChanged += () => EmitSignal(SignalName.FrameChanged, CurrentFrameIndex);
            renderer.Play();
            defaultPos = Position;
            defaulScale = Scale;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);
            if (!MouseDetector.IsHovered && IsFocused && Input.IsActionJustPressed(InputManager.UI_CLICK))
                UnfocusMountain();
        }

        protected override void OnClick()
        {
            if (MouseDetector.IsHovered && !IsFocused)
            {
                IsFocused = true;
                AnimFrameIndex = CurrentFrameIndex;
                SignalBus.Instance.EmitSignal(SignalBus.SignalName.DisplayLevelButton, defaultDisplayedLevel);
            }
            base.OnClick();
        }

        public void RotateToButton(LevelButtonDisplayer pButton)
        {
            renderer.Pause();

            Tween lTween = CreateTween();
            lTween.SetParallel();
            lTween.TweenProperty(this, TweenProp.SCALE, focusScale, AppearAnimDuration);
            lTween.TweenProperty(this, TweenProp.POSITION, (defaultPos - pButton.PosOnMountain) * focusScale, AppearAnimDuration);
            lTween.TweenProperty(this, nameof(AnimFrameIndex), pButton.DisplayFrame, AppearAnimDuration);
        }

        private void UnfocusMountain()
        {
            IsFocused = false;
            renderer.Play();
            Tween lTween = CreateTween();
            lTween.SetParallel();
            lTween.TweenProperty(this, TweenProp.SCALE, defaulScale, AppearAnimDuration);
            lTween.TweenProperty(this, TweenProp.POSITION, defaultPos, AppearAnimDuration);
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.DisplayLevelButton, LevelButtonDisplayer.DISPLAY_NO_LEVEL);
        }
    }
}