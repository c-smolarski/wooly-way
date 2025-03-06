using Godot;
using System;
using static Godot.OpenXRInterface;

// Author : Alissa Delattre

namespace Com.IsartDigital.ProjectName
{

    public partial class Transition : AnimationPlayer
    {
        //Exports for shake settings
        [Export] private float maxShakeRight = 553f;
        [Export] private float minShakeLeft = 597f;
        [Export] private float maxShakeUp = 308f;
        [Export] private float minShakeDown = 335f;

        [Export] private Camera2D camera;
        private Vector2 posCamera;

        private RandomNumberGenerator rand = new RandomNumberGenerator();

        public static bool shake;

        //how long and how often the screen shakes
        private int shakeTime = 0;
        private int shakeFactor = 2;
        private int shakeMaxTime = 130;
        public override void _Ready()
        {
            rand.Randomize();
            posCamera = camera.GlobalPosition;
        }

        public override void _Process(double pDelta)
        {
            float lDelta = (float)pDelta;
            if (shake) IsShaking();
        }

        private void IsShaking()
        {
            shakeTime += 1;

            //using shakeTime as a timer to shake every time shakeTime increments of ten, and checks the shake doesnt last longer then the maximum time 
            if (shakeTime % shakeFactor == 0 && shakeTime < shakeMaxTime)
            {
                camera.Position = new Vector2(rand.RandfRange(maxShakeRight, minShakeLeft), rand.RandfRange(minShakeDown, maxShakeUp));
            }
            //if the shake is done, all initialized settings are reset to their original value
            else if (shakeTime >= shakeMaxTime)
            {
                camera.Position = posCamera;
                shakeTime = 0;
                shake = false;
            }
        }
    }
}
