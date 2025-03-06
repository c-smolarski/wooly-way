using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Godot;
using System;

// Author : Camille SMOLARSKI

namespace Com.IsartDigital.WoolyWay.Managers
{
    // /!\ DO NOT ADD AS NODE, IT IS ALREADY AN AUTOLOAD!
    public partial class InputManager : Node
    {
        [Signal] public delegate void MoveInputPressedEventHandler(Vector2I pDirection);

        public const string MOVE_UP = "move_up";
        public const string MOVE_DOWN = "move_down";
        public const string MOVE_LEFT = "move_left";
        public const string MOVE_RIGHT = "move_right";

        public const string MOVE_PATHFIND = "move_pathfind";

        public const string MENU_PAUSE = "ui_pause";

        public const string UI_BACK = "ui_back";
        public const string UI_CLICK = "ui_click";

        public const string MOVE_UNDO = "move_undo";
        public const string MOVE_REDO = "move_redo";
        
        private const float JOYSTICK_THRESHOLD = 0.75f;

        private static bool isUpPressed, isDownPressed, isLeftPressed, isRightPressed;
        private Vector2I moveInputDirection, lastJoyDir;

        public static InputManager Instance { get; private set; }

        public override void _Ready()
        {
            #region Singleton
            if (Instance != null)
            {
                GD.Print("Error : " + nameof(InputManager) + " already exists. The new one is being freed...");
                QueueFree();
                return;
            }
            Instance = this;
            #endregion
            base._Ready();
        }

        public override void _Process(double pDelta)
        {
            base._Process(pDelta);
            moveInputDirection = GetMovementDirection();
            if (moveInputDirection != Vector2I.Zero)
                Instance.EmitSignal(SignalName.MoveInputPressed, moveInputDirection);
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            base._UnhandledInput(@event);
            isUpPressed = Input.IsActionJustPressed(MOVE_UP);
            isDownPressed = Input.IsActionJustPressed(MOVE_DOWN);
            isLeftPressed = Input.IsActionJustPressed(MOVE_LEFT);
            isRightPressed = Input.IsActionJustPressed(MOVE_RIGHT);
        }

        /// <summary>
        /// Retruns a direction representing the input direction retruned by either keyboard or joypad input.
        /// </summary>
        private Vector2I GetMovementDirection()
        {
            Vector2I lDirection = default;
            if (isUpPressed || isDownPressed || isLeftPressed || isRightPressed)
            {
                lDirection = GetKeyDirection();
                isUpPressed = isDownPressed = isLeftPressed = isRightPressed = false;
            }
            else if (Input.IsJoyKnown(0))
            {
                lDirection = GetJoyStickDirection(0, JoyAxis.LeftX, JoyAxis.LeftY);
                if (lastJoyDir == Vector2I.Zero)
                    lastJoyDir = lDirection;
                else if (lDirection != default)
                    lDirection = default;
                else
                    lastJoyDir = default;
            }

            if(lDirection.X != 0 && lDirection.Y != 0)
                return default;
            return lDirection;
        }

        private static Vector2I GetKeyDirection()
        {
            return new Vector2I(
                isLeftPressed ? -1 : isRightPressed ? 1 : 0,
                isUpPressed ? -1 : isDownPressed ? 1 : 0
            );
        }

        /// <summary></summary>
        /// <param name="pDevice"></param>
        /// <param name="pXAxis"></param>
        /// <param name="pYAxis"></param>
        /// <returns>A Vector2I that represents the direction in which the Joystick is oriented</returns>
        private static Vector2I GetJoyStickDirection(int pDevice, JoyAxis pXAxis, JoyAxis pYAxis)
        {
            return new Vector2I(
                GetAxisSign(Input.GetJoyAxis(pDevice, pXAxis)),
                GetAxisSign(Input.GetJoyAxis(pDevice, pYAxis))
            );
        }

        private static int GetAxisSign(float pAxisValue)
        {
            return (pAxisValue * pAxisValue) >= JOYSTICK_THRESHOLD * JOYSTICK_THRESHOLD ? Mathf.Sign(pAxisValue) : 0;
        }

        protected override void Dispose(bool pDisposing)
        {
            if (Instance == this)
                Instance = null;
            base.Dispose(pDisposing);
        }
    }
}
