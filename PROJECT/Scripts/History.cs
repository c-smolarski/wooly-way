using Com.IsartDigital.WoolyWay.GameObjects;
using Com.IsartDigital.WoolyWay.GameObjects.Mobiles;
using Com.IsartDigital.WoolyWay.Managers;
using Com.IsartDigital.WoolyWay.Utils.TwoWayDictionnaries;
using Com.IsartDigital.WoolyWay.Utils;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// Author : Clément DUCROQUET

namespace Com.IsartDigital.WoolyWay
{
	public partial class History: Control
	{
        #region EXPORTS
        [Export] Button undoButton;
        [Export] Button redoButton;
		[Export] Button restartButton;

        [Export] Label undoText;
        [Export] Label redoText;
        [Export] Label restartText;

        [Export] Sprite2D undoKey;
        [Export] Sprite2D redoKey;

        [Export] Sprite2D undoPanel;
        [Export] Sprite2D redoPanel;
        [Export] Sprite2D restartPanel;
        [Export] Sprite2D pillarPanel;

        [Export] Texture2D[] undoTextures;
        [Export] Texture2D[] redoTextures;
        [Export] Texture2D[] restartTextures;
        [Export] Texture2D[] pillarTextures;

        [Export] Node2D grounds;
        [Export] Node2D items;
        #endregion

        #region INPUT MAPPING STRINGS
        private const string UNDO_KEY = "move_undo";
		private const string REDO_KEY = "move_redo";
        #endregion

        #region HISTORY MANAGEMENT
        static public List<History> historyInstances = new List<History>();

        private List<List<object>> doHistory = new List<List<object>>(); //The main list storing all ordered steps in time direction when doing or redoing
		private List<List<object>> undoHistory = new List<List<object>>(); //The list storing undid steps in reversed order

		private List<object> step;

		private Dictionary<Sheep, Vector2I> sheepProperties;
        private Vector2I playerDirection;

        private Sheep sheep;
        private Player player;

        private bool isClickable = default;
        private bool isLoaded = default; //Checks if the current instance of history is fully loaded with animations and ready to be clicked on.
        #endregion

        #region JUICINESS
        private Vector2[] originalButtonPanelScales;
        private Vector2[] originalButtonScales;
        private Vector2[] originalButtonPanelTextScales;
        private Vector2[] originalButtonPanelKeyScales;

        private Sprite2D[] buttonPanels;
        private Sprite2D[] pillarPanels;

        private Array[] buttonTextures;
        private Array[] pillarTexturesArray;

        private float[] groundDelays = new float[2] { .3f, .6f };
        private float[] itemDelays = new float[6] { 0f, .02f, .04f, .06f, .08f, .1f };
        private float pillarDelay;
        private float[] panelDelays = new float[3] { 0f, .1f, .2f };
        #endregion

        public override void _Ready()
        {
            base._Ready();
            historyInstances.Add(this);

            originalButtonPanelScales = new Vector2[3] { undoPanel.Scale, redoPanel.Scale, restartPanel.Scale };
            originalButtonScales = new Vector2[3] {undoButton.Scale, redoButton.Scale, restartButton.Scale };
            originalButtonPanelTextScales = new Vector2[3] { undoText.Scale, redoText.Scale, restartText.Scale };
            originalButtonPanelKeyScales = new Vector2[2] { undoKey.Scale, redoKey.Scale };

            buttonPanels = new Sprite2D[3] { undoPanel, redoPanel, restartPanel };
            pillarPanels = new Sprite2D[1] { pillarPanel };

            buttonTextures = new Array[3] { undoTextures, redoTextures, restartTextures };
            pillarTexturesArray = new Array[1] { pillarTextures };

            #region BUTTON JUICINESS LINKS
            undoButton.Pressed += () => OnClick(undoPanel, undoButton, undoText, 0, undoTextures, undoKey);
            undoButton.ButtonDown += () => OnButtonDown(undoPanel, undoButton, undoText, 0, undoTextures, undoKey);
            undoButton.MouseEntered += () => OnMouseEnter(undoPanel, undoButton, undoText, 0, undoTextures, undoKey);
            undoButton.MouseExited += () => OnMouseExit(undoPanel, undoButton, undoText, 0, undoTextures, undoKey);

            redoButton.Pressed += () => OnClick(redoPanel, redoButton, redoText, 1, redoTextures, redoKey);
            redoButton.ButtonDown += () => OnButtonDown(redoPanel, redoButton, redoText, 1, redoTextures, redoKey);
            redoButton.MouseEntered += () => OnMouseEnter(redoPanel, redoButton, redoText, 1, redoTextures, redoKey);
            redoButton.MouseExited += () => OnMouseExit(redoPanel, redoButton, redoText, 1, redoTextures, redoKey);

            restartButton.Pressed += () => OnClick(restartPanel, restartButton, restartText, 2, restartTextures);
            restartButton.ButtonDown += () => OnButtonDown(restartPanel, restartButton, restartText, 2, restartTextures);
            restartButton.MouseEntered += () => OnMouseEnter(restartPanel, restartButton, restartText, 2, restartTextures);
            restartButton.MouseExited += () => OnMouseExit(restartPanel, restartButton, restartText, 2, restartTextures);
            #endregion

            //Init
            AddHistoryStep();
            SignalBus.Instance.LevelLoaded += OnAppear; //
            SignalBus.Instance.LevelRestarted += ResetHistory; //Bug from Daniel but no problem with current functionalities (ResetHistory receives triggers twice)
            SignalBus.Instance.LevelFinished += OnDisappear; //
            SignalBus.Instance.PausingMenuDisplayed += ChangeButtonVisibility; //Pause
            SignalBus.Instance.TutorialChangedVisibility += OnTutorialVisibilityChange; //Tuto
            LevelManager.CurrentLevel.FinishedTutorialGrid += OnTutorialChange; //

			undoButton.Pressed += () => PerformHistory(doHistory, undoHistory);
			redoButton.Pressed += () => PerformHistory(undoHistory, doHistory);
			restartButton.Pressed +=  ResetHistory;
            restartButton.Pressed += SendRestartSignal;

            Visible = false;
            OnDisable(false);

            pillarDelay = groundDelays[groundDelays.Count() - 1] + .3f;

            int lGroundCount = grounds.GetChildCount();
            int lItemsCount = items.GetChildCount();

            List<Vector2> lGroundScales = new List<Vector2>();
            List<Vector2> lItemScales = new List<Vector2>();
            Vector2 lPillarScale;

            for (int i = 0;  i < lGroundCount; i++)
            {
                Sprite2D lCurrentGround = grounds.GetChild<Sprite2D>(i);
                lGroundScales.Add(lCurrentGround.Scale);
                lCurrentGround.Scale = Vector2.Zero;
            }

            lPillarScale = pillarPanel.Scale;
            pillarPanel.Scale = Vector2.Zero;

            undoPanel.Scale = Vector2.Zero;
            undoKey.Scale = Vector2.Zero;
            undoText.Scale = Vector2.Zero;

            redoPanel.Scale = Vector2.Zero;
            redoKey.Scale = Vector2.Zero;
            redoText.Scale = Vector2.Zero;

            restartPanel.Scale = Vector2.Zero;
            restartText.Scale = Vector2.Zero;

            for (int i = 0; i < lItemsCount; i++)
            {
                Sprite2D lCurrentItem = items.GetChild<Sprite2D>(i);
                lItemScales.Add(lCurrentItem.Scale);
                lCurrentItem.Scale = Vector2.Zero;
            }

            Visible = true;

            Tween lTween = CreateTween().SetParallel().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);

            for (int i = 0; i < lGroundCount; i++)
            {
                Sprite2D lCurrentGround = grounds.GetChild<Sprite2D>(i);
                lTween.TweenProperty(lCurrentGround, "scale", lGroundScales[i], .35f).SetDelay(groundDelays[i]);
            }

            lTween.TweenProperty(pillarPanel, "scale", lPillarScale, .35f).SetDelay(pillarDelay);

            lTween.Chain().TweenProperty(undoPanel, "scale", originalButtonPanelScales[0], .35f).SetDelay(panelDelays[0]);
            lTween.TweenProperty(undoKey, "scale", originalButtonPanelKeyScales[0], .35f).SetDelay(panelDelays[0]);
            lTween.TweenProperty(undoText, "scale", originalButtonPanelTextScales[0], .35f).SetDelay(panelDelays[0]);

            lTween.TweenProperty(redoPanel, "scale", originalButtonPanelScales[1], .35f).SetDelay(panelDelays[1]);
            lTween.TweenProperty(redoKey, "scale", originalButtonPanelKeyScales[1], .35f).SetDelay(panelDelays[1]);
            lTween.TweenProperty(redoText, "scale", originalButtonPanelTextScales[1], .35f).SetDelay(panelDelays[1]);

            lTween.TweenProperty(restartPanel, "scale", originalButtonPanelScales[2], .35f).SetDelay(panelDelays[2]);
            lTween.TweenProperty(restartText, "scale", originalButtonPanelTextScales[2], .35f).SetDelay(panelDelays[2]);

            for (int i = 0; i < lItemsCount; i++)
            {
                Sprite2D lCurrentItem = items.GetChild<Sprite2D>(i);
                if (i == 0) lTween.Chain().TweenProperty(lCurrentItem, "scale", lItemScales[i], .35f).SetDelay(itemDelays[i]);
                else lTween.TweenProperty(lCurrentItem, "scale", lItemScales[i], .35f).SetDelay(itemDelays[i]);
            }

            lTween.Finished += () =>
            {
                OnDisable(true);
                isLoaded = true;
            };
        }

        public override void _Process(double delta)
        {
            if (Player.CurrentlyMoving) return;

            bool lIsUndoing = Input.IsActionJustPressed(UNDO_KEY, true);
            bool lIsRedoing = Input.IsActionJustPressed(REDO_KEY, true);

            if (lIsUndoing && lIsRedoing) return;

            if (lIsUndoing && !undoButton.Disabled) PerformHistory(doHistory, undoHistory);
            else if (lIsRedoing && !redoButton.Disabled) PerformHistory(undoHistory, doHistory);
        }

        /*
         * JUICINESS
         */

        private void OnAppear()
        {
            OnDisable(isLoaded);
        }

        private void OnDisappear()
        {
            OnDisable(false);
            DisableButtons(true);
        }

        private void ChangeTexture(uint pIndex, Sprite2D[] pSprites, Array[] pTextures)
        {
            int lLength = pSprites.Length;

            for (int lItemIndex = 0; lItemIndex < lLength; lItemIndex++)
                pSprites[lItemIndex].Texture = ((Texture2D[])pTextures[lItemIndex])[pIndex];
        }

        private void OnDisable(bool pIsClickable)
        {
            isClickable = pIsClickable;
            DisableButtons(!pIsClickable);

            if (isClickable)
            {
                ChangeTexture(0, buttonPanels, buttonTextures);
                ChangeTexture(0, pillarPanels, pillarTexturesArray);
            }
            else
            {
                ChangeTexture(3, buttonPanels, buttonTextures);
                ChangeTexture(1, pillarPanels, pillarTexturesArray);
            }
        }

        private void OnMouseEnter(Sprite2D pButtonPanel, Button pButton, Label pButtonText, uint pIndex, Texture2D[] pPanelArray, Sprite2D pButtonKey = null)
        {
            if (isClickable)
            {
                pButtonPanel.Texture = pPanelArray[1];
                Tween lTween = CreateTween().SetParallel().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
                lTween.TweenProperty(pButtonPanel, "scale", originalButtonPanelScales[pIndex] + originalButtonPanelScales[pIndex] * .05f, .08f);
                lTween.TweenProperty(pButton, "scale", originalButtonScales[pIndex] + originalButtonScales[pIndex] * .05f, .08f);
                lTween.TweenProperty(pButtonText, "scale", originalButtonPanelTextScales[pIndex] + originalButtonPanelTextScales[pIndex] * .05f, .08f);

                if (pButtonKey != null)
                    lTween.TweenProperty(pButtonKey, "scale", originalButtonPanelKeyScales[pIndex] + originalButtonPanelKeyScales[pIndex] * .05f, .08f);
            }
        }

        private void OnMouseExit(Sprite2D pButtonPanel, Button pButton, Label pButtonText, uint pIndex, Texture2D[] pPanelArray, Sprite2D pButtonKey = null)
        {
            if (isClickable)
            {
                pButtonPanel.Texture = pPanelArray[0];
                Tween lTween = CreateTween().SetParallel().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
                lTween.TweenProperty(pButtonPanel, "scale", originalButtonPanelScales[pIndex], .08f);
                lTween.TweenProperty(pButton, "scale", originalButtonScales[pIndex], .08f);
                lTween.TweenProperty(pButtonText, "scale", originalButtonPanelTextScales[pIndex], .08f);

                if (pButtonKey != null)
                    lTween.TweenProperty(pButtonKey, "scale", originalButtonPanelKeyScales[pIndex], .08f);
            }
        }

        private void OnButtonDown(Sprite2D pButtonPanel, Button pButton, Label pButtonText, uint pIndex, Texture2D[] pPanelArray, Sprite2D pButtonKey = null)
        {
            if (isClickable)
            {
                pButtonPanel.Texture = pPanelArray[2];
                Tween lTween = CreateTween().SetParallel().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quart);
                lTween.TweenProperty(pButtonPanel, "scale", originalButtonPanelScales[pIndex] - originalButtonPanelScales[pIndex] * .15f, .08f);
                lTween.TweenProperty(pButton, "scale", originalButtonScales[pIndex] - originalButtonScales[pIndex] * .15f, .08f);
                lTween.TweenProperty(pButtonText, "scale", originalButtonPanelTextScales[pIndex] - originalButtonPanelTextScales[pIndex] * .15f, .08f);

                if (pButtonKey != null)
                    lTween.TweenProperty(pButtonKey, "scale", originalButtonPanelKeyScales[pIndex] - originalButtonPanelKeyScales[pIndex] * .15f, .08f);
            }
        }

        private void OnClick(Sprite2D pButtonPanel, Button pButton, Label pButtonText, uint pIndex, Texture2D[] pPanelArray, Sprite2D pButtonKey = null)
        {
            if (isClickable)
            {
                pButtonPanel.Texture = pPanelArray[1];

                SoundManager.GetInstance().PlaySFXFromArray(SoundManager.GetInstance().UIButtonClick);

                Tween lTween = CreateTween().SetParallel().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
                lTween.TweenProperty(pButtonPanel, "scale", originalButtonPanelScales[pIndex] + originalButtonPanelScales[pIndex] * .05f, .45f);
                lTween.TweenProperty(pButton, "scale", originalButtonScales[pIndex] + originalButtonScales[pIndex] * .05f, .45f);
                lTween.TweenProperty(pButtonText, "scale", originalButtonPanelTextScales[pIndex] + originalButtonPanelTextScales[pIndex] * .05f, .45f);

                if (pButtonKey != null)
                    lTween.TweenProperty(pButtonKey, "scale", originalButtonPanelKeyScales[pIndex] + originalButtonPanelKeyScales[pIndex] * .05f, .45f);
            }
        }

        /*
         * ON SIGNAL RECEPTION METHODS
         */

        private void OnTutorialChange(Grid pOldGrid)
        {
            pOldGrid.FinishedTutorialGrid -= OnTutorialChange;
            if(IsInstanceValid(LevelManager.CurrentLevel))
                LevelManager.CurrentLevel.FinishedTutorialGrid += OnTutorialChange;
            SetCurrentDictAsDefault();
            OnDisable(true);
            EnableButtons();
        }

        /// <summary>
        /// Hides buttons if game is paused.
        /// </summary>
        private void ChangeButtonVisibility()
        {
            if (IsInsideTree())
            {
                //GD.Print(GetTree().Paused ? "HIDDING" : "SHOWING");
                Visible = !GetTree().Paused;
            }
        }

        /// <summary>
        /// Change interactibility of buttons according to the FTUE state (open/closed).
        /// </summary>
        private void OnTutorialVisibilityChange(bool pVisible) //pVisible: visibility of FTUE frame, not button visibility
        {
            OnDisable(!pVisible);

            if(pVisible)
                SignalBus.Instance.LevelLoaded -= OnAppear;
            else if (!IsQueuedForDeletion())
                SignalBus.Instance.LevelLoaded += OnAppear;
        }

        private void EnableButtons()
        {
            OnDisable(true);
        }

        private void DisableButtons()
        {
            OnDisable(false);
        }

        /// <summary>
        /// Enables or disables all buttons depending on the specified parameter.
        /// </summary>
        private void DisableButtons(bool pDisabled) //
        {
            if(IsInstanceValid(undoButton) && IsInstanceValid(redoButton) && IsInstanceValid(restartButton))
                undoButton.Disabled = redoButton.Disabled = restartButton.Disabled = pDisabled;
        }

        /*
         * RESET HISTORY METHODS
         */

        /// <summary>
        /// Empty lists history lists and resets the original step.
        /// </summary>
        private void SetCurrentDictAsDefault()
        {
            doHistory.Clear(); //Resets the time directed history
            if(IsInstanceValid(LevelManager.CurrentLevel)) AddHistoryStep(); //Then re-adds the orginal step (that has just been performed)
        }

        /// <summary>
        /// Restores the original round state based init data.
        /// </summary>
		private void ResetHistory()
		{
            if (Player.CurrentlyMoving || LevelManager.CurrentLevel.SheepList.Any(lSheep => lSheep.IsRotating))
                return;

            step = doHistory[0];
            PerformChanges(step); //Performs changes based on the original step (saved on init)
            SetCurrentDictAsDefault();
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.LevelLoaded);
        }

        /*
         * STEP METHODS
         */

        /// <summary>
        /// Returns the most recent grid and sheep directions into a list of objects at the current game state.
        /// </summary>
        /// <returns></returns>
        private List<object> GetCurrentStep()
        {
            TwoWayDictionary<Tile, GameObject> lRoundCurrentState = new TwoWayDictionary<Tile, GameObject>();
            sheepProperties = new Dictionary<Sheep, Vector2I>();

            foreach (KeyValuePair<Tile, GameObject> lObject in LevelManager.CurrentLevel.ObjectDict)
            {
                lRoundCurrentState.Add(lObject);

                if (lObject.Value is Player)
                {
                    player = (Player)lObject.Value;
                    playerDirection = player.Direction;
                }
                else if (lObject.Value is Sheep)
                {
                    sheep = (Sheep)lObject.Value;
                    sheepProperties.Add(sheep, sheep.Direction);
                }
            }

            return new List<object>() { lRoundCurrentState.ToReadOnly(), sheepProperties, playerDirection };
        }

        /// <summary>
		/// Called when a step is going to be processed. Refreshes history line of the current round.
		/// </summary>
        public void AddHistoryStep()
		{
            step = GetCurrentStep();
            doHistory.Add(step);

			undoHistory.Clear(); //Each time a step is added, it's not possible to attempt any redo action that would challenge the game rules logic.
		}

        private void SendRestartSignal()
        {
            if(!LevelManager.isTutorial)
                SignalBus.Instance.EmitSignal(SignalBus.SignalName.LevelRestarted);
        }

        private void SendUndoRedoSignal(bool isUndo)
        { 
            if(!LevelManager.isTutorial)
                SignalBus.Instance.EmitSignal(SignalBus.SignalName.UndoRedoPar, isUndo);
        }

        /*
         * APPLY CHANGES METHODS
         */

        /// <summary>
        /// Performs all required logic and visual changed to process any undo, redo or restart action.
        /// </summary>
        /// <param name="pListToRemoveFrom"></param>
        /// <param name="pListToAddTo"></param>
        private void PerformHistory(List<List<object>> pListToRemoveFrom, List<List<object>> pListToAddTo)
		{
            #region DECLARATIONS

            int lStepIndex;
            bool lCanPerform;

            #endregion
                        
            if (Player.CurrentlyMoving || LevelManager.CurrentLevel.SheepList.Any(lSheep => lSheep.IsRotating)) return;

            lStepIndex = pListToRemoveFrom.Count - 1;
            lCanPerform = pListToRemoveFrom == doHistory ? lStepIndex >= 1 : lStepIndex >= 0;

			if (lCanPerform)
			{
                step = GetCurrentStep();
                pListToAddTo.Add(step); //Takes the current step before performing any changes.

                PerformChanges(pListToRemoveFrom[lStepIndex]); //Performs changes on last saved element of pListToRemoveFrom once current state is saved (into step).
                pListToRemoveFrom.RemoveAt(lStepIndex); //Then removes the last step (that has just been performed).
                SendUndoRedoSignal(pListToRemoveFrom == doHistory);
            }
        }

        /// <summary>
        /// Applies logic and visual changes according to the requested action.
        /// </summary>
        /// <param name="pStep"></param>
		private void PerformChanges(List<object> pStep)
        {
            ReadOnlyTwoWayDictionary<Tile, GameObject> lNewGrid = (ReadOnlyTwoWayDictionary<Tile, GameObject>)pStep[0];
            sheepProperties = (Dictionary<Sheep, Vector2I>)pStep[1];
            playerDirection = (Vector2I)pStep[2];

            LevelManager.CurrentLevel.ObjectDict = lNewGrid; //Applying logic changes (the current grid is replaced).

            foreach (GameObject lObject in lNewGrid.Values)
            {
                if (lObject is Mobile)
                {
                    lObject.UpdatePos(); //But also visual changes (all Mobiles are refreshed according the new logic grid).

                    if (lObject is Player)
                    {
                        player = (Player)lObject;
                        player.Direction = playerDirection; //Logic changes
                        StringDirection.SetSpriteDirection(playerDirection, player.Renderer); //Visual changes
                    }
                    else if (lObject is Sheep)
                    {
                        sheep = (Sheep)lObject;
                        foreach (KeyValuePair<Sheep, Vector2I> lSheepProperties in sheepProperties)
                        {
                            if (lSheepProperties.Key == sheep)
                            {
                                sheep.Direction = lSheepProperties.Value; //Logic changes
                                StringDirection.SetSpriteDirection(lSheepProperties.Value, sheep.Renderer); //Visual changes
                                break;
                            }
                        }
                    }
                }
            }
        }

        protected override void Dispose(bool pDisposing)
        {
            if (historyInstances.Contains(this))
                historyInstances.Remove(this);

            SignalBus.Instance.LevelLoaded -= OnAppear;
            SignalBus.Instance.LevelRestarted -= ResetHistory;
            SignalBus.Instance.LevelFinished -= OnDisappear;
            SignalBus.Instance.PausingMenuDisplayed -= ChangeButtonVisibility;
            SignalBus.Instance.TutorialChangedVisibility -= OnTutorialVisibilityChange;
            if(IsInstanceValid(restartButton))
                restartButton.Pressed -= ResetHistory;

            if(IsInstanceValid(LevelManager.CurrentLevel))
                LevelManager.CurrentLevel.FinishedTutorialGrid -= OnTutorialChange;
            base.Dispose(pDisposing);
        }
    }
}