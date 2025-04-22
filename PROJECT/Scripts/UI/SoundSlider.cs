using Godot;
using System;

// Author : SMOLARSKI Camille

namespace Com.IsartDigital.WoolyWay.UI
{
    public partial class SoundSlider : HSlider
    {
        [Export] private string audioBusName;
        [Export] private Label labelValue;

        private int busIndex;

        public override void _Ready()
        {
            base._Ready();
            busIndex = AudioServer.GetBusIndex(audioBusName);
            SliderInit();
            ValueChanged += OnGrabberMoved;
        }

        private void SliderInit()
        {
            MinValue = 0;
            MaxValue = 2;
            Step = 0.01f;
            Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(busIndex));
            OnGrabberMoved(Value);
        }

        private void OnGrabberMoved(double pNewValue)
        {
            AudioServer.SetBusVolumeDb(busIndex, (float)Mathf.LinearToDb(pNewValue));
            labelValue.Text = "" + Mathf.RoundToInt(pNewValue * 100);
        }
    }
}
