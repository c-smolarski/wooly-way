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

        protected override float AnimStepDuration => 0.25f;
        private Vector2 TrunkBouncyScale => trunk.Scale * new Vector2(0.5f, 1.5f);

        private static readonly Vector2 trunkHiddenScale = Vector2.Right;
        private static readonly Vector2 leavesHiddenScale = Vector2.Zero;

        private List<Transform2D> baseTransforms;

        public override void _Ready()
        {
            base._Ready();
            BaseTransformsInit();
        }

        private void BaseTransformsInit()
        {
            baseTransforms = new();
            baseTransforms.Add(trunk.Transform);
            foreach (Sprite2D lLeaf in leaves)
                baseTransforms.Add(lLeaf.Transform);
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
            Visible = false;
            trunk.Transform = baseTransforms[0];
            foreach (Sprite2D lLeaf in leaves)
                lLeaf.Transform = baseTransforms[Array.IndexOf(leaves, lLeaf) + 1];
        }
    }
}
