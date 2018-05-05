using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class Player : MonoBehaviour
{
    #region Delegate Types
    /// <summary>
    /// OnFinish Delegate Type.
    /// </summary>
    public delegate void OnFinishDelegate();
    #endregion

    #region Properties
    #region Public
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

    #region Protected
    /// <summary>
    /// Current Score.
    /// </summary>
    protected int currentScore = 0;
    /// <summary>
    /// Current Score.
    /// </summary>
    public int CurrentScore
    {
        get
        {
            return currentScore;
        }
    }
    /// <summary>
    /// List of current Cards.
    /// </summary>
    protected List<Card> currentCards;
    /// <summary>
    /// True if the player has a BlackJack, False otherwise.
    /// </summary>
    protected bool isBlackJack = false;
    /// <summary>
    /// True if the player has busted, False otherwise.
    /// </summary>
    protected bool isBusted = false;
    /// <summary>
    /// Is the player Busted?
    /// </summary>
    public bool IsBusted
    {
        get
        {
            return isBusted;
        }
    }
    #endregion
    #endregion

    #region Events from Inspector
    /// <summary>
    /// Event to invoke when the turn starts.
    /// </summary>
    public UnityEvent OnStartTurn;
    /// <summary>
    /// Event to invoke when the turn finishes.
    /// </summary>
    public UnityEvent OnFinishTurn;
    #endregion

    #region Methods
    #region MonoBehaviour Methods
    /// <summary>
    /// Component Awake Method.
    /// </summary>
    private void Awake()
    {
        // Initializing currentCards.
        currentCards = new List<Card>();
    }
    #endregion

    #region Public
    /// <summary>
    /// Resets the player's infos and sends its cards back to the Deck.
    /// </summary>
    /// <returns>IEnumerator value.</returns>
    public virtual IEnumerator ResetInfos()
    {
        currentScore = 0;
        isBusted = false;
        isBlackJack = false;

        // Sends back the cards.
        yield return StartCoroutine(GameManager.StaticDeckRef.ReaddCards_Coroutine(currentCards));

        currentCards.Clear();

        currentScoreText.text = "0";
        currentSituationText.text = "";

        yield return null;
    }
    /// <summary>
    /// Sets the turn started for this player, invoking its events.
    /// </summary>
    public virtual void StartTurn()
    {
        if (OnStartTurn != null) OnStartTurn.Invoke();
    }
    /// <summary>
    /// Makes the player get the card.
    /// </summary>
    /// <param name="card">The card to get.</param>
    public virtual void GetCard(Card card)
    {
        // The card is added to the current cards list.
        currentCards.Add(card);

        // Recalculating the score...
        int newScore = 0;
        foreach (Card c in currentCards)
        {
            newScore += c.cardPrimaryScoreValue;
        }

        // Setting special situations...
        if (newScore == GameManager.BlackJackPoints && currentCards.Count < 3)
        {
            currentSituationText.text = "JACK BLACK!";
            isBlackJack = true;
        }
        else if (newScore > GameManager.BlackJackPoints)
        {
            currentSituationText.text = "Soft";
            
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
                        newScore += c.cardSecondaryScoreValue;
                    }
                    else
                    {
                        newScore += c.cardPrimaryScoreValue;
                    }
                }
            }
        }

        currentScore = newScore;

        currentScoreText.text = currentScore.ToString();

        // Set the correct position to the card.
        SetCardPosition(card);

        // Check if the player busted anyway.
        if (currentScore > GameManager.BlackJackPoints)
        {
            Bust();
        }
    }
    /// <summary>
    /// Indicates that the player doesn't want more cards.
    /// </summary>
    public virtual void Stop()
    {
        currentSituationText.text = "Stop.";

        Finish();
    }
    #endregion

    #region Protected
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
        currentSituationText.text = "Busted!";

        isBusted = true;
        Finish();
    }
    /// <summary>
    /// Ends the turn for this player, invoking its events.
    /// </summary>
    protected virtual void Finish()
    {
        if (OnFinishTurn != null) OnFinishTurn.Invoke();

        if (OnFinish != null) OnFinish();
    }
    #endregion
    #endregion
}
