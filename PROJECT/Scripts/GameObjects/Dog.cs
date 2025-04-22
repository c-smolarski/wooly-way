using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils.Tweens;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using static Godot.OpenXRInterface;
using Com.IsartDigital.WoolyWay.Components;


// Author : Tom Bagnara

namespace Com.IsartDigital.WoolyWay.GameObjects
{
	public partial class Dog : Obstacle
	{
		[Export] private Node2D barkPolygonContainer;
		[Export] private AnimatedSprite2D Renderer;
        [Export] private ClickableArea clickable;
        [Export] private bool isAsset = false;


		private const float BARK_ANIM_DURATION = 0.5f;
		private const float BARK_ANIM_STRENGTH = 1.3f;

		private Vector2I Direction;

		private Vector2 baseScale;

        private Tween tween;
        private Vector2 tweenValues = new Vector2();
        private bool breathOut = false;
        private bool isBarking = false;
        private float breathTarget = .115f;
        private RandomNumberGenerator rand = new RandomNumberGenerator();

        public override void _Ready()
        {
            if(!isAsset)
                base._Ready();
            else
            {
                GetChild<Polygon2D>(0).Visible = false;
                barkPolygonContainer.Position = new Vector2(barkPolygonContainer.Position.X * -1, barkPolygonContainer.Position.Y);
                barkPolygonContainer.Scale = new Vector2(barkPolygonContainer.Scale.X * -1, barkPolygonContainer.Scale.Y);
            }

			baseScale = Renderer.Scale;
            tweenValues = new Vector2(breathTarget, Renderer.Scale.X);
            Timer timer = new Timer();
            timer.WaitTime = rand.RandfRange(0f, 1.5f);
            timer.OneShot = true;
            AddChild(timer);
            timer.Start();
            timer.Timeout += () => DogBreath(tweenValues.X);
            clickable.Clicked += Bark;
        }

        private void DogBreath(float pTarget)
        {
            tween = CreateTween();
            tween.TweenProperty(Renderer, TweenProp.SCALE, new Vector2(Renderer.Scale.X, pTarget), 1.5f);
            tween.Finished += NextBreath;
        }

        private void NextBreath()
        {
            breathOut = !breathOut;
            if (breathOut)
            {
                DogBreath(tweenValues.Y);
                return;
            }
            DogBreath(tweenValues.X);
        }

        private void InitDirection(Tile pSpawnTile)
		{
            Tile lTargetTile = Grid.IndexDict[Grid.IndexDict[pSpawnTile] + Direction];
            lTargetTile?.SetDog(this);
        }

		public void Bark()
		{
            if(isBarking) return;
            isBarking = true;
            SoundManager.GetInstance().PlaySFXFromArray(SoundManager.GetInstance().DogSFXList);
			Tween lTween = CreateTween();
			lTween.TweenProperty(Renderer, TweenProp.SCALE, baseScale, BARK_ANIM_DURATION)
				.From(Renderer.Scale * BARK_ANIM_STRENGTH);
			lTween.Parallel().TweenProperty(barkPolygonContainer, TweenProp.MODULATE_ALPHA, 0, BARK_ANIM_DURATION)
				.From(1);
            lTween.Finished += CantBark;
        }

        public void CantBark()
        {
            isBarking = false;
        }

		public static Dog Create(PackedScene pScene, Tile pTile, Vector2I pDirection)
		{
			Dog lDog = Create<Dog>(pScene, pTile);
			lDog.Direction = pDirection.Sign();
			lDog.InitDirection(pTile);
			StringDirection.SetSpriteDirection(lDog.Direction, lDog.Renderer);
			return lDog;
		}

		private void UpdateSprite()
		{
			StringDirection.SetSpriteDirection(Direction, Renderer);
			barkPolygonContainer.Scale = new Vector2(Renderer.FlipH ? 1 : -1, barkPolygonContainer.Scale.Y);
			barkPolygonContainer.Position = new Vector2(Mathf.Abs(barkPolygonContainer.Position.X) * (Renderer.FlipH ? 1 : -1), barkPolygonContainer.Position.Y);
		}
		
    }
}
