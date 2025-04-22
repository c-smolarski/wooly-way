using Com.IsartDigital.WoolyWay.Data;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;
using System.Collections.Generic;

// Author : Daniel Moussouni--Lepilliez

namespace Com.IsartDigital.ProjectName {
	
	public partial class DogMenu : TextureButton
	{
        private const float BARK_ANIM_DURATION = 0.5f;
        private const float BARK_ANIM_STRENGTH = 1.3f;
        private const string IDLE_BACK = "IdleBack";
        private const string IDLE_FRONT = "IdleFront";
        [Export] private Node2D barkPolygonContainer;
        private Vector2 baseScale;
        private static List<AudioStreamOggVorbis> soundList = new();
        protected static AudioStreamOggVorbis appearSFX;
        static DogMenu()
        {
            FilePath.FetchAllFromFile(out soundList, SoundPath.DOG_BARK_FOLDER);
        }
        public override void _Ready()
        {
            Pressed += Bark;
            baseScale = Scale;
        }
        private void Bark()
        {
            PlaySound(soundList[new Random().Next(soundList.Count)]);
            Tween lTween = CreateTween();
            lTween.TweenProperty(this, TweenProp.SCALE, baseScale, BARK_ANIM_DURATION)
                .From(baseScale *  BARK_ANIM_STRENGTH);
            lTween.Parallel().TweenProperty(barkPolygonContainer, TweenProp.MODULATE_ALPHA, 0, BARK_ANIM_DURATION)
                .From(1);
        }
        protected AudioStreamPlayer PlaySound(AudioStreamOggVorbis pStream)
        {
            AudioStreamPlayer lPlayer = new AudioStreamPlayer();
            lPlayer.Stream = pStream;
            lPlayer.Bus = SoundPath.SFX_SOUND_BUS;
            GetTree().Root.AddChild(lPlayer);
            lPlayer.Finished += () => lPlayer.QueueFree();
            lPlayer.Play();
            return lPlayer;
        }

        public override void _Process(double pDelta)
		{
			float lDelta = (float)pDelta;

		}

		protected override void Dispose(bool pDisposing)
		{

		}
	}
}
