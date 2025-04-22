using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.Managers;
using Godot;
using Com.IsartDigital.WoolyWay.Utils;
using System;
using System.Reflection.Metadata.Ecma335;

// Author : Camille SMOLARSKI & Tom BAGNARA

namespace Com.IsartDigital.WoolyWay.GameObjects
{
    public abstract partial class Mobile : GameObject
    {
        [Signal] public delegate void FinishedMovingEventHandler();
        [Export] public AnimatedSprite2D Renderer;

        public const float MOVE_DURATION = 0.25f;
        private const float ZINDEX_THRESHOLD = 0.2f;
        
        protected virtual bool IsMoving { get; set; } = false;

        private bool zindexAlreadySet;
        private float elapsedTime = 0;

        public Vector2I Direction { get; set; }

        public override void _Process(double pDelta)
        {
            base._Process(pDelta);
            if (IsMoving)
                Move((float)pDelta);
        }

        private void Move(float pDelta)
        {
            elapsedTime += pDelta;
            float lProgress = elapsedTime / MOVE_DURATION;

            if(!zindexAlreadySet && lProgress > ZINDEX_THRESHOLD)
            {
                zindexAlreadySet = true;
                SetZIndex(CurrentTile);
            }

            if (lProgress > 1)
            {
                IsMoving = false;
                elapsedTime = 0;
                zindexAlreadySet = false;
                EmitSignal(SignalName.FinishedMoving);
                return;
            }
            Position = Position.Lerp(CurrentTile.Position, lProgress);
            SoundManager.GetInstance().PlaySFXFromArray(SoundManager.GetInstance().FootstepsSFX, MOVE_DURATION);
        }

        /// <summary>
        /// Checks if current object can move in the specified direction by verifying if the next tile is walkable.
        /// If the next tile contains a sheep and is not blocked by another object, it moves it out of the way.
        /// </summary>
        /// <param name="pMoveDirection"></param>
        public virtual void InitMove(Vector2I pMoveDirection)
        {
            Tile lNextTile = Grid.IndexDict[CurrentTile.Index + pMoveDirection];

            if (!lNextTile.IsWalkable(pMoveDirection)) return;

            if (this is Player)
            {
                StringDirection.SetSpriteDirection(pMoveDirection, Renderer, StringDirection.WALK);
                Direction = pMoveDirection;
                //checks if needs to add step to tutorial or the level for undo/redo
                if (History.historyInstances.Count > 1) History.historyInstances[1].AddHistoryStep();
                else History.historyInstances[0].AddHistoryStep();
            }
            

            if (!lNextTile.IsEmpty() && lNextTile.IsSheep())
            { 
                Sheep lSheep = lNextTile.Object as Sheep;
                if (this is Sheep && lSheep.Direction == -pMoveDirection) return;
                
                MoveTo(lNextTile);

                lSheep?.InitMove(pMoveDirection);
            }
            else
                MoveTo(lNextTile);
        }

        protected virtual void MoveTo(Tile pTile)
        {
            Grid.ObjectDict[this] = pTile;
            IsMoving = true;
        }
    }
}
