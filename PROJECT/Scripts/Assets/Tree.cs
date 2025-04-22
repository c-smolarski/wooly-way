using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Godot;
using System;
using System.Collections.Generic;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.Assets
{
    public partial class Tree : Asset
    {
        [Export] private Sprite2D trunk;
        [Export] private Sprite2D[] leaves;
        [Export] private GpuParticles2D leavesParticles;

        private float LeafMaxAngle => Mathf.DegToRad(3f);
        private float TreeMaxAngle => Mathf.DegToRad(2f);

        protected override float AnimStepDuration => 0.25f;
        private Vector2 TrunkBouncyScale => trunk.Scale * new Vector2(0.5f, 1.5f);

        private static readonly Vector2 trunkHiddenScale = Vector2.Right;
        private static readonly Vector2 leavesHiddenScale = Vector2.Zero;

        private List<Transform2D> baseTransforms;
        private Dictionary<Sprite2D, float> leavesAngleTarget = new();
        private float angleTarget;

        public override void _Ready()
        {
            base._Ready();
            BaseTransformsInit();
            if(leavesParticles != null)
            {
                ParticleProcessMaterial lParticleaMat = (ParticleProcessMaterial)(leavesParticles.ProcessMaterial = (Material)leavesParticles.ProcessMaterial.Duplicate());
                lParticleaMat.ScaleMin *= Mathf.Min(GlobalScale.X, GlobalScale.Y);
                lParticleaMat.ScaleMax *= Mathf.Min(GlobalScale.X, GlobalScale.Y);
                leavesParticles.Emitting = false;
            }
        }

        private void BaseTransformsInit()
        {
            baseTransforms = new() { trunk.Transform };
            foreach (Sprite2D lLeaf in leaves)
            {
                baseTransforms.Add(lLeaf.Transform);
                lLeaf.Rotation = GD.RandRange(-1, 1) * LeafMaxAngle;
                leavesAngleTarget.Add(lLeaf, (Mathf.Sign(lLeaf.Rotation) == 0 ? 1f : Mathf.Sign(lLeaf.Rotation)) * LeafMaxAngle);
            }


            Rotation = GD.RandRange(-1, 1) * TreeMaxAngle;
            angleTarget = (Mathf.Sign(Rotation) == 0 ? 1f : Mathf.Sign(Rotation)) * TreeMaxAngle;
        }

        public override void _Process(double pDelta)
        {
            base._Process(pDelta);
            if (Visible)
            {
                foreach (Sprite2D lLeaf in leaves)
                {
                    if (lLeaf.Rotation * lLeaf.Rotation < LeafMaxAngle * LeafMaxAngle)
                        lLeaf.Rotation = Mathf.Clamp(lLeaf.Rotation + ((float)pDelta * 0.0375f) * Mathf.Sign(leavesAngleTarget[lLeaf]), -LeafMaxAngle, LeafMaxAngle);
                    else
                    {
                        leavesAngleTarget[lLeaf] = leavesAngleTarget[lLeaf] * -1;
                        lLeaf.Rotation = (Mathf.Abs(lLeaf.Rotation) - 0.001f) * Mathf.Sign(lLeaf.Rotation);
                    }
                }

                if (Rotation * Rotation < TreeMaxAngle * TreeMaxAngle)
                    Rotation = Mathf.Clamp(Rotation + ((float)pDelta * 0.075f) * Mathf.Clamp(1 - (Mathf.Abs(Rotation) / TreeMaxAngle), 0.1f, 0.5f) * Mathf.Sign(angleTarget), -TreeMaxAngle, TreeMaxAngle);
                else
                {
                    angleTarget = angleTarget * -1;
                    Rotation = (Mathf.Abs(Rotation) - 0.001f) * Mathf.Sign(Rotation);
                }

            }
                
        }

        public override void PlayAppearAnim()
        {
            Visible = true;
            Tween lTween = CreateTween();

            foreach (Sprite2D lLeaf in leaves)
                lTween.TweenProperty(lLeaf, TweenProp.SCALE, leavesHiddenScale, 0f);

            lTween.TweenProperty(trunk, TweenProp.SCALE, TrunkBouncyScale, AnimStepDuration)
                    .From(trunkHiddenScale);
            lTween.TweenProperty(trunk, TweenProp.SCALE, trunk.Scale, AnimStepDuration);

            lTween.SetParallel(true);

            foreach (Sprite2D lLeaf in leaves)
                lTween.TweenProperty(lLeaf, TweenProp.SCALE, lLeaf.Scale, AnimStepDuration * 2f)
                    .SetEase(Tween.EaseType.Out)
                    .SetTrans(Tween.TransitionType.Bounce)
                    .SetDelay(AnimStepDuration * 0.5f * Array.IndexOf(leaves, lLeaf));

            if (leavesParticles != null)
                lTween.Finished += () => leavesParticles.Emitting = true;

        }

        public override void PlayDisappearAnim()
        {
            Tween lTween = CreateTween();
            foreach (Sprite2D lLeaf in leaves)
                lTween.Parallel().TweenProperty(lLeaf, TweenProp.SCALE, leavesHiddenScale, AnimStepDuration);

            lTween.TweenProperty(trunk, TweenProp.SCALE, TrunkBouncyScale, AnimStepDuration);
            lTween.TweenProperty(trunk, TweenProp.SCALE, trunkHiddenScale, AnimStepDuration);
            lTween.Finished += ResetValues;
        }

        private void ResetValues()
        {
            if (leavesParticles != null)
                leavesParticles.Emitting = false;

            Visible = false;
            trunk.Transform = baseTransforms[0];
            foreach (Sprite2D lLeaf in leaves)
                lLeaf.Transform = baseTransforms[Array.IndexOf(leaves, lLeaf) + 1];
        }
    }
}
