using System.Collections;
using UnityEngine;
using CabbageSoft.BlackJack.Characters;
using CabbageSoft.BlackJack.DeckManagement;

namespace CabbageSoft.BlackJack
{
    public class PlayerAI : Player
    {
        #region Inspector Infos
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

        [SerializeField] private PlayerAIMatchInfoController playerAIMatchInfoControllerPrefab = default;
        #endregion

        #region Private Stuff
        private DataManager.PlayerAIInitData playerAIData = default;
        private PlayerAIMatchInfoController playerAIMatchInfoController = default;
        private CharacterModelController characterModelController = default;
        #endregion

        #region Public Methods
        public void Initialize(PlayersManager playersManager, DataManager.PlayerAIInitData data)
        {
            playerAIData = data;

            // Initializing "playerAIMatchInfoController".
            playerAIMatchInfoController = Instantiate(playerAIMatchInfoControllerPrefab, playersManager.topPanelGroup);

            nameText = playerAIMatchInfoController.NameText;
            currentScoreText = playerAIMatchInfoController.CurrentScoreText;
            currentSituationText = playerAIMatchInfoController.CurrentSituationText;
            
            // Initializing "characterModelController".
            characterModelController = Instantiate(playerAIData.CharacterAIProperties.CharacterModelPrefab, modelPivotTransform);

            // Populating variables from data.
            playerName = playerAIData.CharacterAIProperties.CharacterName;
            percentageOfRisk = playerAIData.CharacterAIProperties.RiskPercentage;
            minValueForRiskCalculation = playerAIData.CharacterAIProperties.RiskCalcMinScore;

            // Visual update.
            nameText.text = playerName;

            characterModelController.SetFaceSprite(playerAIData.CharacterAIProperties.FrontFaceSprite);
        }

        /// <summary>
        /// Resets the player's infos and sends its cards back to the Deck.
        /// </summary>
        /// <returns>IEnumerator value.</returns>
        public override IEnumerator ResetInfos()
        {
            // Default ResetInfos functions...
            yield return base.ResetInfos();

            playerAIMatchInfoController.ShowDialogue(string.Empty, 0f);
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
            if (!IsBusted)
            {
                MakeDecision();
            }
            else
            {
                playerAIMatchInfoController.ShowDialogue(playerAIData.CharacterAIProperties.DialogueStringBusted, 2f);
            }
        }
        /// <summary>
        /// Indicates that the player doesn't want more cards.
        /// </summary>
        public override void Stop()
        {
            characterModelController.SetTriggerAction(CharacterModelController.ECharacterAction.Stop);

            playerAIMatchInfoController.ShowDialogue(playerAIData.CharacterAIProperties.DialogueStringDone, 2f);

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
             * 3) The Dealer has busted and the player hasn't.
             */

            // If the player is busted, it doesn't check the score.
            if (IsBusted) return;

            if (isDealerBlackJack)
            {
                // If the dealer has a BlackJack, the player has lost the turn.
                SetLower();
            }
            else if (isBlackJack || CurrentScore > dealerScore || dealerScore > GameManager.BlackJackPoints)
            {
                SetHigher();
            }
            else
            {
                SetLower();
            }
        }
        #endregion

        #region Protected Methods
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
            CurrentState = EState.Done;
            
            // Resets the face's scale.
            characterModelController.SetFaceScale(false);

            StartCoroutine(Finish_Coroutine());

            IEnumerator Finish_Coroutine()
            {
                yield return new WaitForSeconds(2f);

                // Default Finish functions...
                base.Finish();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Manages the player's decisions.
        /// </summary>
        private void MakeDecision()
        {
            // If has a BlackJack, the player stops.
            if (CurrentScore == GameManager.BlackJackPoints)
            {
                Stop();
                return;
            }

            // If has not reached the minimum score for risk calculation, the player asks one more card.
            if (CurrentScore < minValueForRiskCalculation)
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

            playerAIMatchInfoController.ShowDialogue(string.Format(playerAIData.CharacterAIProperties.DialogueStringGiveCard, currentCards.Count + 1), -1f);
        }
        /// <summary>
        /// Sets the player as a Winner.
        /// </summary>
        private void SetHigher()
        {
            characterModelController.SetTriggerAction(CharacterModelController.ECharacterAction.Win);
            playerAIMatchInfoController.ShowDialogue(playerAIData.CharacterAIProperties.DialogueStringMatchWon, 2f);
            
            CurrentSituation = EScoreSituation.Won;
        }
        /// <summary>
        /// Sets the player as a Loser.
        /// </summary>
        private void SetLower()
        {
            characterModelController.SetTriggerAction(CharacterModelController.ECharacterAction.Lose);
            playerAIMatchInfoController.ShowDialogue(playerAIData.CharacterAIProperties.DialogueStringMatchLost, 2f);

            CurrentSituation = EScoreSituation.Lost;
        }
        #endregion
    }
}
