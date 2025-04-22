using Com.IsartDigital.WoolyWay.Data;
using Godot;
using System;
using System.Collections.Generic;

// Author : SMOLARSKI Camille && Julien Lim

namespace Com.IsartDigital.WoolyWay.UI.Buttons
{
	public abstract partial class AbstractButton : BaseButton
	{
		[ExportGroup("Override base sounds")]
		[Export] private AudioStreamOggVorbis[] overrideSounds;

		private static List<AudioStreamOggVorbis> soundList = new();
		protected static AudioStreamOggVorbis appearSFX;

		static AbstractButton()
		{
			FilePath.FetchAllFromFile(out soundList, SoundPath.UI_BUTTON_SFX_FOLDER);
			appearSFX = GD.Load<AudioStreamOggVorbis>(SoundPath.UI_BUTTON_APPEAR_SFX);
		}

        public override void _Ready()
		{
			base._Ready();
			Pressed += OnPressed;
		}

        protected virtual void OnPressed()
        {
            if (overrideSounds == null || overrideSounds.Length == 0)
				PlaySound(soundList[new Random().Next(soundList.Count)]);
			else
				PlaySound(overrideSounds[new Random().Next(overrideSounds.Length)]);
        }

		protected AudioStreamPlayer PlaySound(AudioStreamOggVorbis pStream)
		{
			AudioStreamPlayer lPlayer = new AudioStreamPlayer();
			lPlayer.Stream = pStream;
			lPlayer.Bus = SoundPath.SFX_SOUND_BUS;
			lPlayer.ProcessMode = ProcessModeEnum.Always;
			GetTree().Root.AddChild(lPlayer);
			lPlayer.Finished += () => lPlayer.QueueFree();
			lPlayer.Play();
			return lPlayer;
		}

        public virtual void Disable()
        {
            if (HasFocus())
                ReleaseFocus();
            Disabled = true;
        }
    }
}
