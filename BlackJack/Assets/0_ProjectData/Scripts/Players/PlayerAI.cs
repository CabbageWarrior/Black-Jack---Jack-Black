using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CabbageSoft.BlackJack.Characters;

namespace CabbageSoft.BlackJack
{
    public class PlayerAI : Player
    {
        #region Properties
        #region Public
        public Transform modelPivotTransform = default;

        /// <summary>
        /// Percentage of risk when the score is greater than the minimum score for risk calculation.
        /// </summary>
        [Header("AI")]
        [Range(0f, 100f)]
        public float percentageOfRisk = 0f;
        /// <summary>
        /// Minimum score for risk calculation.
        /// </summary>
        [Range(0, 20)]
        public int minValueForRiskCalculation = 0;
        /// <summary>
        /// Reference to the DecisionCanvas.
        /// </summary>
        public GameObject decisionCanvas;
        /// <summary>
        /// Reference to the CurrentDecisionText.
        /// </summary>
        public Text currentDecisionText;

        [Space]
        [SerializeField] private Transform panelToTopTransform = default;
        #endregion

        #region Private
        /// <summary>
        /// Reference to the coroutines that manage the DecisionPanel.
        /// </summary>
        private Coroutine manageDecisionPanelCoroutine;

        private CharacterModelController characterModelController = default;
        #endregion
        #endregion

        #region Methods
        #region Public
        public void Initialize(PlayersManager playersManager, DataManager.PlayerAIInitData data)
        {
            characterModelController = Instantiate(data.CharacterAIProperties.CharacterModelPrefab, modelPivotTransform);

            playerName = data.CharacterAIProperties.CharacterName;
            percentageOfRisk = data.CharacterAIProperties.RiskPercentage;
            minValueForRiskCalculation = data.CharacterAIProperties.RiskCalcMinScore;

            // Visual update.
            nameText.text = playerName;

            characterModelController.SetFaceSprite(data.CharacterAIProperties.FrontFaceSprite);

            if (panelToTopTransform)
            {
                panelToTopTransform.parent = playersManager.topPanelGroup;
                
                panelToTopTransform.localPosition = Vector3.zero;
                panelToTopTransform.localRotation = Quaternion.identity;
                panelToTopTransform.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// Sets the rotation of InfoCanvas.
        /// </summary>
        public void SetInfoCanvasRotation()
        {
            // The canvas must be in the same direction of the Main Camera.
            decisionCanvas.transform.rotation = Camera.main.transform.rotation;
        }

        /// <summary>
        /// Resets the player's infos and sends its cards back to the Deck.
        /// </summary>
        /// <returns>IEnumerator value.</returns>
        public override IEnumerator ResetInfos()
        {
            // Default ResetInfos functions...
            yield return StartCoroutine(base.ResetInfos());

            HideDecisionPanelAfterSecs(0f);

            yield return null;
        }
        /// <summary>
        /// Sets the turn started for this player, invoking its events.
        /// </summary>
        public override void StartTurn()
        {
            // Default StartTurn functions...
            base.StartTurn();

            // Scales the face to a greater value in order to declare who is the current player.
            characterModelController.SetFaceScale(true);

            MakeDecision();
        }
        /// <summary>
        /// Makes the player get the card.
        /// </summary>
        /// <param name="card">The card to get.</param>
        public override void GetCard(Card card)
        {
            // Default GetCard functions...
            base.GetCard(card);

            // Check the new situation and decide what to do.
            if (!isBusted)
            {
                MakeDecision();
            }
            else
            {
                decisionCanvas.SetActive(true);
                currentDecisionText.text = "Holy crap!";
                HideDecisionPanelAfterSecs(2f);
            }
        }
        /// <summary>
        /// Indicates that the player doesn't want more cards.
        /// </summary>
        public override void Stop()
        {
            characterModelController.SetTriggerAction(CharacterModelController.ECharacterAction.Stop);

            decisionCanvas.SetActive(true);
            currentDecisionText.text = "Ok, I'm done.";
            HideDecisionPanelAfterSecs(2f);

            // Default Stop functions...
            base.Stop();
        }
        /// <summary>
        /// Checks the score compared to the dealer's situation.
        /// </summary>
        /// <param name="dealerScore">Dealer's score.</param>
        /// <param name="isDealerBlackJack">Dealer's BlackJack situation.</param>
        public void CheckScore(int dealerScore, bool isDealerBlackJack)
        {
            /* The player wins in these cases:
             * 1) The player has a BlackJack and the Dealer doesn't.
             * 2) The player's current score is higher than the Dealer's.
             * 3) The Dealer has busted.
             */

            // If the player is busted, it doesn't check the score.
            if (isBusted) return;

            if (isDealerBlackJack)
            {
                // If the dealer has a BlackJack, the player has lost the turn.
                SetLower();
            }
            else if (isBlackJack || currentScore > dealerScore || dealerScore > GameManager.BlackJackPoints)
            {
                SetHigher();
            }
            else
            {
                SetLower();
            }
        }
        #endregion

        #region Protected
        /// <summary>
        /// Makes the player bust.
        /// </summary>
        protected override void Bust()
        {
            characterModelController.SetTriggerAction(CharacterModelController.ECharacterAction.Busted);

            // Default Bust functions...
            base.Bust();
        }
        /// <summary>
        /// Ends the turn for this player, invoking its events.
        /// </summary>
        protected override void Finish()
        {
            // Resets the face's scale.
            characterModelController.SetFaceScale(false);

            // Default Finish functions...
            base.Finish();
        }
        #endregion

        #region Private
        /// <summary>
        /// Manages the player's decisions.
        /// </summary>
        private void MakeDecision()
        {
            // If has a BlackJack, the player stops.
            if (currentScore == GameManager.BlackJackPoints)
            {
                Stop();
                return;
            }

            // If has not reached the minimum score for risk calculation, the player asks one more card.
            if (currentScore < minValueForRiskCalculation)
            {
                AskCard();
                return;
            }

            // Check with a Random if the player wants a card or not.
            if (Random.Range(0f, 100f) <= percentageOfRisk)
            {
                AskCard();
            }
            else
            {
                Stop();
            }
        }
        /// <summary>
        /// Asks one more card.
        /// </summary>
        private void AskCard()
        {
            characterModelController.SetTriggerAction(CharacterModelController.ECharacterAction.AskCard);

            decisionCanvas.SetActive(true);
            currentDecisionText.text = "Gimme my card number " + (currentCards.Count + 1).ToString() + "!";
        }
        /// <summary>
        /// Sets the player as a Winner.
        /// </summary>
        private void SetHigher()
        {
            characterModelController.SetTriggerAction(CharacterModelController.ECharacterAction.Win);
            decisionCanvas.SetActive(true);
            currentDecisionText.text = "YEEEEEAH!";
            HideDecisionPanelAfterSecs(2f);

            currentSituationText.text = "Win!";
        }
        /// <summary>
        /// Sets the player as a Loser.
        /// </summary>
        private void SetLower()
        {
            characterModelController.SetTriggerAction(CharacterModelController.ECharacterAction.Lose);
            decisionCanvas.SetActive(true);
            currentDecisionText.text = "Oh no!";
            HideDecisionPanelAfterSecs(2f);

            currentSituationText.text = "Lose...";
        }

        /// <summary>
        /// Hides the DecisionPanel after the seconds specified.
        /// </summary>
        /// <param name="secs">Seconds after which the panel disappears.</param>
        private void HideDecisionPanelAfterSecs(float secs)
        {
            // If a coroutine is already in progress, the system stops it in order to avoid strange behaviours.
            if (manageDecisionPanelCoroutine != null) StopCoroutine(manageDecisionPanelCoroutine);

            manageDecisionPanelCoroutine = StartCoroutine(HideDecisionPanelAfterSecs_Coroutine(secs));
        }
        /// <summary>
        /// Hides the DecisionPanel after the seconds specified.
        /// </summary>
        /// <param name="secs">Seconds after which the panel disappears.</param>
        /// <returns>IEnumerator value.</returns>
        private IEnumerator HideDecisionPanelAfterSecs_Coroutine(float secs)
        {
            yield return new WaitForSeconds(secs);

            decisionCanvas.SetActive(false);
            currentDecisionText.text = string.Empty;

            yield return null;
        }
        #endregion
        #endregion
    }
}
