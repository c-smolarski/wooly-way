using Godot;
using System;
using Com.IsartDigital.WoolyWay.Assets.SingleAssets;

// Author : Clément DUCROQUET

namespace Com.IsartDigital.ProjectName
{
	public partial class Mill : Bush
	{
        #region EXPORTS
        [Export] private Node2D smoke;
        [Export] private Node2D rotorPale;
        [Export] private Node2D rotorBody;

        [Export] private Sprite2D mill;
        [Export] private Sprite2D pales;
        [Export] private Texture2D[] millBodySprites;
        [Export] private Texture2D[] millPaleSprites;
        #endregion

        private bool isSmoking = default;

        private Vector2 millDefaultScale;
        private Vector2 smokeDefaultPosition;

        private uint frameIndex = default;

        RandomNumberGenerator rand = new RandomNumberGenerator();

        #region JUICINESS VARIABLES
        private int circleMaxRotation = 360;

        private float delayMin = .02f;
        private float delayMax = 3.6f;

        private float rotationMin = .45f;
        private int rotationMax = 4;

        private float durationByCircle = 6.2f;
        private float tweenDuration = .5f;

        private float stretchSize = .005f;

        private float frameCount = 60f;
        private float frameRotationMax = 90f;
        private float degBetweenFrame;

        private const string rotationDegreesProperty = "rotation_degrees";
        #endregion

        public override void _Ready()
        {
            base._Ready();
            rand.Randomize();

            millDefaultScale = Scale;
            smokeDefaultPosition = smoke.GlobalPosition;
            degBetweenFrame = frameRotationMax / frameCount;

            EnableSmoke(false);
        }

        public override void _Process(double delta)
        {
            base._Process(delta);

            if (!isSmoking && isSpawned)
            {
                isSmoking = true;
                EnableSmoke(true);
                SquashAndStretch();
                OnPaleRotate();
                OnMillRotate();
            }

            smoke.GlobalPosition = smokeDefaultPosition;
            RotationToAnimation(pales, millPaleSprites, rotorPale);
            RotationToAnimation(mill, millBodySprites, rotorBody);
        }

        private void OnPaleRotate()
        {
            float lDelay = rand.RandfRange(delayMin, delayMax);
            float lRotationDegrees = rand.RandiRange((int)(rotationMin * circleMaxRotation), rotationMax * circleMaxRotation);
            float lDuration = durationByCircle / circleMaxRotation * lRotationDegrees;

            Tween lRotorTween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quint);
            lRotorTween.TweenProperty(rotorPale, rotationDegreesProperty, rotorPale.RotationDegrees + lRotationDegrees, lDuration).SetDelay(lDelay);

            lRotorTween.Finished += OnPaleRotate;
        }

        private void OnMillRotate()
        {
            float lDelay = rand.RandfRange(2.6f, 5.4f);
            float lRotationDegrees = rand.RandiRange((int)(.12f * circleMaxRotation), (int)(.32f * circleMaxRotation));
            float lDuration = 5.1f / circleMaxRotation * lRotationDegrees;

            Tween lRotorTween = CreateTween().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
            lRotorTween.TweenProperty(rotorBody, rotationDegreesProperty, rotorBody.RotationDegrees + lRotationDegrees, lDuration).SetDelay(lDelay);

            lRotorTween.Finished += OnMillRotate;
        }

        private void RotationToAnimation(Sprite2D pSprite, Texture2D[] pTextures, Node2D pRotor)
        {
            int lCurrentRotation = (int)pRotor.RotationDegrees % (int)frameRotationMax;
            bool lReadyToSwitch = Mathf.FloorToInt(lCurrentRotation % degBetweenFrame) == default;
            int lCurrentIndex = Mathf.FloorToInt(lCurrentRotation / degBetweenFrame);

            if (lReadyToSwitch && frameIndex != lCurrentIndex)
            {
                pSprite.Texture = pTextures[lCurrentIndex];
                frameIndex = (uint)lCurrentIndex;
            }
        }

        private void EnableSmoke(bool pEmit)
        {
            int lSmokeEmitterCount = smoke.GetChildCount();
            GpuParticles2D lEmitter;
            
            for (int lChildIndex = default; lChildIndex < lSmokeEmitterCount; lChildIndex++)
            {
                lEmitter = smoke.GetChild<GpuParticles2D>(lChildIndex);
                lEmitter.Emitting = pEmit;
            }
        }

        private void SquashAndStretch()
        {
            Tween lBuildingTween = CreateTween().SetLoops().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
            lBuildingTween.TweenProperty(this, "scale:y", millDefaultScale.Y - stretchSize, tweenDuration);
            lBuildingTween.TweenProperty(this, "scale:y", millDefaultScale.Y, tweenDuration);
        }
    }
}
