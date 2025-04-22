using Com.IsartDigital.WoolyWay.UI.Menus;
using Com.IsartDigital.WoolyWay.Data;
using Godot;
using System;

// Author : Alissa Delattre && Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.UI
{
    public partial class Transition : AnimationPlayer
    {
        //Exports for shake settings
        [Export] private float maxShakeStrength = 30f;
        [Export] private bool transIn;
        [Export] private Camera2D camera;

        private const string RESET_ANIM = "RESET";
        private const string ANIMATION_NAME = "transition";
        private const string SPRITE_PATH = "Sprite2D";


        public static bool shake;
        private static readonly Vector2 spriteMargin = new Vector2(100, 0);
        private static AudioStreamPlayer transitionSoundPlayer;

        private bool exportedCamera;
        private Vector2 posCamera;
        private Sprite2D sprite;
        private RandomNumberGenerator rand = new RandomNumberGenerator();

        //how long and how often the screen shakes
        private float shakeTime, shakeFactor = 2;
        private float elapsedTime, shakeMaxTime;

        public override void _Ready()
        {
            shakeMaxTime = GetAnimation(ANIMATION_NAME).Length;
            sprite = GetNode<Sprite2D>(SPRITE_PATH);
            sprite.Visible = false;
            rand.Randomize();
            CameraInit();
            TransitionPlayerInit();
            if (!transIn)
                Start(transIn);
        }

        public override void _Process(double pDelta)
        {
            float lDelta = (float)pDelta;
            if (shake)
                IsShaking(lDelta);
        }

        private void CameraInit()
        {
            if (camera == null)
            {
                camera = GetViewport().GetCamera2D();
                posCamera = camera.Position;
                GetViewport().SizeChanged += UpdateAnimPos;
                UpdateAnimPos();
            }
            else
            {
                exportedCamera = true;
                camera.MakeCurrent();
                GetViewport().SizeChanged += UpdateCameraPos;
                UpdateCameraPos();
            }
        }

        private void TransitionPlayerInit()
        {
            if (transitionSoundPlayer == null)
            {
                transitionSoundPlayer = new AudioStreamPlayer();
                transitionSoundPlayer.Stream = GD.Load<AudioStreamOggVorbis>(SoundPath.UI_TRANSITION_SOUND_PATH);
                transitionSoundPlayer.Autoplay = false;
                transitionSoundPlayer.Bus = SoundPath.SFX_SOUND_BUS;
                GetTree().Root.CallDeferred("add_child", transitionSoundPlayer);
            }
        }

        public void Start(bool pIn = true)
        {
            transIn = pIn;
            sprite.Visible = true;
            posCamera = camera.Position;
            UpdateAnimPos();
            Play(ANIMATION_NAME);
            shake = true;
        }

        private void IsShaking(float pDelta)
        {
            elapsedTime += pDelta;
            shakeTime++;

            float lTransMult = transIn ? elapsedTime / shakeMaxTime : (shakeMaxTime - elapsedTime) / shakeMaxTime;
            
            if (elapsedTime < shakeMaxTime)
            {
                //using shakeTime as a timer to shake every time shakeTime increments of ten, and checks the shake doesnt last longer then the maximum time 
                if (shakeTime % shakeFactor == 0)
                    camera.Position = posCamera + lTransMult * new Vector2(rand.RandfRange(-maxShakeStrength, maxShakeStrength), rand.RandfRange(-maxShakeStrength, maxShakeStrength));

                if (!transitionSoundPlayer.Playing)
                    transitionSoundPlayer.Play();
                transitionSoundPlayer.VolumeDb = Mathf.LinearToDb(lTransMult);
            }
            //if the shake is done, all initialized settings are reset to their original value
            else
            {
                camera.Position = posCamera;
                shakeTime = elapsedTime = 0;
                shake = false;
                if (!transIn)
                {
                    transitionSoundPlayer.Stop();
                    CurrentAnimation = RESET_ANIM;
                }

                GetTree().CreateTimer(0.1f, false).Timeout += () => sprite.Visible = false;
            }
        }

        private void UpdateCameraPos()
        {
            posCamera = camera.Position = GetViewport().GetVisibleRect().Size / 2;
            UpdateAnimPos();
        }

        private void UpdateAnimPos()
        {
            Vector2 lStartPoint = posCamera - (transIn ? new Vector2(sprite.Texture.GetWidth(), 0) * sprite.Scale + spriteMargin : Vector2.Zero);
            sprite.Position = lStartPoint;
            Animation lAnim = GetAnimation(ANIMATION_NAME);
            lAnim.TrackSetKeyValue(0, 0, lStartPoint);
            lAnim.TrackSetKeyValue(0, 1, posCamera + (transIn ? Vector2.Zero : new Vector2(sprite.Texture.GetWidth(), 0) * sprite.Scale + spriteMargin));
            GetAnimation(RESET_ANIM).TrackSetKeyValue(0, 0, posCamera - new Vector2(sprite.Texture.GetWidth(),0) * sprite.Scale + spriteMargin);
            if(!shake)
                CurrentAnimation = RESET_ANIM;
        }

        protected override void Dispose(bool pDisposing)
        {
            if (IsInstanceValid(GetViewport()))
            {
                if (!exportedCamera)
                    GetViewport().SizeChanged -= UpdateAnimPos;
                else
                    GetViewport().SizeChanged -= UpdateCameraPos;
            }
            base.Dispose(pDisposing);
        }
    }
}