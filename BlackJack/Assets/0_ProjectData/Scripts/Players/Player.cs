using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using CabbageSoft.BlackJack.DeckManagement;

namespace CabbageSoft.BlackJack
{
    public class Player : MonoBehaviour
    {
        public enum EState
        {
            WaitingForCard,
            Done
        }

        public enum EScoreSituation
        {
            Normal,
            Soft,
            BlackJack,
            Stopped,
            Busted,

            Won,
            Lost
        }

        #region Delegate Types
        /// <summary>
        /// OnFinish Delegate Type.
        /// </summary>
        public delegate void OnFinishDelegate();
        #endregion

        #region Inspector Infos
        /// <summary>
        /// Player's name.
        /// </summary>
        public string playerName;

        /// <summary>
        /// Reference to the Text Component of the name.
        /// </summary>
        [Header("Components")]
        public Text nameText;
        /// <summary>
        /// Slot for cards acquired during the match.
        /// </summary>
        public GameObject cardSlot;

        /// <summary>
        /// Reference to the Text Component of the score.
        /// </summary>
        public Text currentScoreText;
        /// <summary>
        /// Reference to the Text Component of the current situation.
        /// </summary>
        public Text currentSituationText;

        /// <summary>
        /// What happens when the player finishes its turn.
        /// </summary>
        public OnFinishDelegate OnFinish = null;
        #endregion

        #region Protected Stuff
        /// <summary>
        /// List of current Cards.
        /// </summary>
        protected List<Card> currentCards = new List<Card>();
        /// <summary>
        /// True if the player has a BlackJack, False otherwise.
        /// </summary>
        protected bool isBlackJack = false;
        #endregion

        #region Private Stuff
        private EScoreSituation currentSituation = EScoreSituation.Normal;
        #endregion

        #region Properties
        /// <summary>
        /// Current Score.
        /// </summary>
        public int CurrentScore { get; protected set; }

        /// <summary>
        /// Is the player Busted?
        /// </summary>
        public bool IsBusted { get; protected set; } = false;

        public EState CurrentState { get; protected set; } = EState.WaitingForCard;
        public EScoreSituation CurrentSituation
        {
            get => currentSituation; protected set
            {
                currentSituation = value;

                switch (currentSituation)
                {
                    case EScoreSituation.Normal:
                        currentSituationText.text = "";
                        break;
                    case EScoreSituation.Soft:
                        currentSituationText.text = "Soft";
                        break;
                    case EScoreSituation.BlackJack:
                        currentSituationText.text = "JACK BLACK!";
                        break;
                    case EScoreSituation.Stopped:
                        currentSituationText.text = "Stop.";
                        break;
                    case EScoreSituation.Busted:
                        currentSituationText.text = "Busted!";
                        break;
                    case EScoreSituation.Won:
                        currentSituationText.text = "Win!";
                        break;
                    case EScoreSituation.Lost:
                        currentSituationText.text = "Lose...";
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Events from Inspector
        /// <summary>
        /// Event to invoke when the turn starts.
        /// </summary>
        public UnityEvent OnStartTurn = default;
        /// <summary>
        /// Event to invoke when the turn finishes.
        /// </summary>
        public UnityEvent OnFinishTurn = default;
        #endregion

        #region Public Methods
        /// <summary>
        /// Resets the player's infos and sends its cards back to the Deck.
        /// </summary>
        /// <returns>IEnumerator value.</returns>
        public virtual IEnumerator ResetInfos()
        {
            CurrentScore = 0;
            IsBusted = false;
            isBlackJack = false;
            CurrentState = EState.WaitingForCard;
            CurrentSituation = EScoreSituation.Normal;

            // Sends back the cards.
            yield return StartCoroutine(GameManager.StaticDeckRef.ReaddCards_Coroutine(currentCards));

            currentCards.Clear();

            currentScoreText.text = CurrentScore.ToString();

            yield return null;
        }
        /// <summary>
        /// Sets the turn started for this player, invoking its events.
        /// </summary>
        public virtual void StartTurn()
        {
            OnStartTurn?.Invoke();
        }
        /// <summary>
        /// Makes the player get the card.
        /// </summary>
        /// <param name="card">The card to get.</param>
        public virtual void GetCard(Card card)
        {
            // The card is added to the current cards list.
            currentCards.Add(card);
            // Set the card as Used.
            card.Use();

            // Recalculating the score...
            int newScore = 0;
            foreach (Card c in currentCards)
            {
                newScore += c.CardPrimaryScoreValue;
            }

            // Setting special situations...
            if (newScore == GameManager.BlackJackPoints && currentCards.Count == 2)
            {
                isBlackJack = true;
            }
            else if (newScore > GameManager.BlackJackPoints)
            {
                CurrentSituation = EScoreSituation.Soft;

                // If there are cards with a secondary value different from the first, check if the player doesn't bust using that.
                // The system checks the secondary value one card ad a time, in order to avoid wrong behaviours in case di multiple aces.
                for (int i = 0; newScore > GameManager.BlackJackPoints && i < currentCards.Count; i++)
                {
                    newScore = 0;
                    for (int j = 0; j < currentCards.Count; j++)
                    {
                        Card c = currentCards[j];

                        if (j <= i)
                        {
                            newScore += c.CardSecondaryScoreValue;
                        }
                        else
                        {
                            newScore += c.CardPrimaryScoreValue;
                        }
                    }
                }
            }

            CurrentScore = newScore;

            currentScoreText.text = CurrentScore.ToString();

            // Set the correct position to the card.
            SetCardPosition(card);

            // Check if the player busted anyway.
            if (CurrentScore > GameManager.BlackJackPoints)
            {
                Bust();
            }
        }
        /// <summary>
        /// Indicates that the player doesn't want more cards.
        /// </summary>
        public virtual void Stop()
        {
            if (isBlackJack)
            {
                CurrentSituation = EScoreSituation.BlackJack;
            }
            else
            {
                CurrentSituation = EScoreSituation.Stopped;
            }

            Finish();
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Sets the new position to the card.
        /// </summary>
        /// <param name="card">Card to move.</param>
        protected virtual void SetCardPosition(Card card)
        {
            // Used DOTween in order to move the card. The new position is based on how many cards the player has.
            card.transform.DOMove(cardSlot.transform.position + new Vector3(0f, .001f * currentCards.Count, -.155f * (currentCards.Count - 1)), .5f);
        }

        /// <summary>
        /// Makes the player bust.
        /// </summary>
        protected virtual void Bust()
        {
            CurrentSituation = EScoreSituation.Busted;

            IsBusted = true;
            Finish();
        }
        /// <summary>
        /// Ends the turn for this player, invoking its events.
        /// </summary>
        protected virtual void Finish()
        {
            CurrentState = EState.Done;

            OnFinishTurn?.Invoke();

            OnFinish?.Invoke();
        }
        #endregion
    }
}
