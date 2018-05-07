using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Deck : MonoBehaviour
{
    #region Properties
    #region Public
    /// <summary>
    /// Reference to the Card Prefab.
    /// </summary>
    public GameObject cardPrefab;
    /// <summary>
    /// Offset amount for cards in deck.
    /// </summary>
    public float cardOffset = 0f;

    /// <summary>
    /// GameObject that highlights the deck when it can be clicked.
    /// </summary>
    [Space]
    public GameObject deckHighlighter;

    /// <summary>
    /// Reference to the current first card of the deck.
    /// </summary>
    [Space]
    public Card firstCard = null;
    #endregion

    #region Private
    /// <summary>
    /// List of all the cards.
    /// </summary>
    private List<Card> allCards;
    /// <summary>
    /// List of all the cards currentlyin the deck.
    /// </summary>
    private List<Card> cardsInDeck;

    /// <summary>
    /// Is the deck managing cards right now?
    /// </summary>
    private bool isManagingCards = false;
    #endregion
    #endregion

    #region Events from Inspector
    /// <summary>
    /// What happens when the deck starts to manage cards.
    /// </summary>
    public UnityEvent OnManagingCardsStart;
    /// <summary>
    /// What happens when the deck finishes to manage cards.
    /// </summary>
    public UnityEvent OnManagingCardsEnd;
    #endregion

    #region Methods
    #region MonoBehaviour Methods
    /// <summary>
    /// Component Start method.
    /// </summary>
    private void Start()
    {
        // Need to spawn the 52 cards.
        SpawnCards();
    }
    #endregion

    #region Public
    /// <summary>
    /// Shuffles the cards that are currently into the deck.
    /// </summary>
    public void ShuffleCardsInDeck()
    {
        if (isManagingCards || GameManager.currentState != GameManager.TurnState.Game) return;

        StartCoroutine(ShuffleCardsInDeck_Coroutine());
    }
    /// <summary>
    /// Manages the deck shuffle.
    /// </summary>
    /// <returns>IEnumerator value.</returns>
    private IEnumerator ShuffleCardsInDeck_Coroutine()
    {
        SetManagingCards(true);
        deckHighlighter.SetActive(false);

        // For each card, the shuffle method swaps it with another random card that is part of the deck.
        int n = cardsInDeck.Count;
        if (n > 0)
        {
            while (n > 1)
            {
                n--;

                // Make a "shuffle visual effect" moving cards on axis X and Z.
                Sequence mySequenceX = DOTween.Sequence();
                Sequence mySequenceZ = DOTween.Sequence();
                mySequenceX
                    .Append(cardsInDeck[n].transform.DOMoveX(transform.position.x + Random.Range(-.2f, .2f), .2f))
                    .Append(cardsInDeck[n].transform.DOMoveX(transform.position.x, .2f));
                mySequenceZ
                    .Append(cardsInDeck[n].transform.DOMoveZ(transform.position.z + Random.Range(-.2f, .2f), .2f))
                    .Append(cardsInDeck[n].transform.DOMoveZ(transform.position.z, .2f));

                // Selecting the random index with which make the swap.
                int k = Random.Range(0, cardsInDeck.Count);

                // Swapping cards.
                Card temp = cardsInDeck[k];
                cardsInDeck[k] = cardsInDeck[n];
                cardsInDeck[n] = temp;
            }

            // Reorder the Y positions.
            SetCardsPositionByOrder();

            // Reset the first card reference after the shuffle.
            ResetFirstCard();
        }

        if (GameManager.currentState == GameManager.TurnState.Game) deckHighlighter.SetActive(true);
        SetManagingCards(false);

        yield return null;
    }

    /// <summary>
    /// Reorders the vertical position of every card based on their sort.
    /// </summary>
    public void SetCardsPositionByOrder()
    {
        for (int i = 0; i < cardsInDeck.Count; i++)
        {
            cardsInDeck[i].transform.position = new Vector3(
                transform.position.x,
                transform.position.y - cardOffset * (i + allCards.Count - cardsInDeck.Count),
                transform.position.z
            );
        }
    }

    /// <summary>
    /// Manages the insertion of the cards from the players' hands back to the deck.
    /// </summary>
    /// <param name="cards">List of cards to reinsert into the deck.</param>
    /// <returns>IEnumerator value.</returns>
    public IEnumerator ReaddCards_Coroutine(List<Card> cards)
    {
        // Wait until the deck is not managing other cards.
        while (isManagingCards) yield return null;

        if (cards != null && cards.Count > 0)
        {
            SetManagingCards(true);
            
            // For each card, reset its state and reinsert it into the deck.
            foreach (Card card in cards)
            {
                card.ReturnIntoDeck();

                yield return new WaitForSeconds(.05f);
            }
            // Clear the player's cards list.
            cards.Clear();

            SetManagingCards(false);
        }

        yield return null;
    }

    /// <summary>
    /// Changes the card management state.
    /// </summary>
    /// <param name="isManaging">True if the deck is starting the management, False otherwise.</param>
    public void SetManagingCards(bool isManaging)
    {
        if (isManaging)
        {
            isManagingCards = true;
            if (OnManagingCardsStart != null) OnManagingCardsStart.Invoke();
        }
        else
        {
            isManagingCards = false;
            if (OnManagingCardsEnd != null) OnManagingCardsEnd.Invoke();
        }
    }

    public void AddCard(Card card)
    {
        cardsInDeck.Add(card);
        
        // Reset Y order.
        SetCardsPositionByOrder();

        // Reset the first card reference.
        ResetFirstCard();
    }
    public Vector3 ReaddCardPosition()
    {
        Vector3 deckInitialPosition = transform.position;
        Vector3 newPositionPoint = deckInitialPosition + new Vector3(0f, cardOffset * (cardsInDeck.Count + 1), 0f);

        return newPositionPoint;
    }

    public void ClickEvent()
    {
        switch (GameManager.instance.DataManager.CardSendingMode)
        {
            case DataManager.CardSendingType.SingleClick:
                OnMouseUpAsButton_Event();
                break;

            case DataManager.CardSendingType.DragAndDrop:
                OnMouseDown_Event();
                break;
        }
    }

    /// <summary>
    /// Overwrites the first card reference with the current first card of the deck.
    /// </summary>
    public void ResetFirstCard()
    {
        if (cardsInDeck.Count > 0)
        {
            firstCard = cardsInDeck[0];
        }
        else
        {
            firstCard = null;
        }
    }
    #endregion

    #region Private
    /// <summary>
    /// Spawns the 52 cards and assigns their single values.
    /// </summary>
    private void SpawnCards()
    {
        SetManagingCards(true);

        // Cards List initialization.
        if (allCards == null)
        {
            allCards = new List<Card>();
        }
        else
        {
            allCards.Clear();
        }

        // Cards go from 2 to 11/1.
        for (int i = 0; i < 52; i++)
        {
            GameObject newCardGO = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);

            Card newCard = newCardGO.GetComponent<Card>();
            newCard.cardIndex = i;

            // Initialize the callback.
            newCard.OnUsed += ResetFirstCard;

            // Check the index for the single suit.
            int suitIndex = i % 13;

            if (suitIndex < 9) // Numbers 2-10
            {
                newCard.cardPrimaryScoreValue = suitIndex + 2;
                newCard.cardSecondaryScoreValue = suitIndex + 2;
            }
            else if (suitIndex < 12) // Figures
            {
                newCard.cardPrimaryScoreValue = 10;
                newCard.cardSecondaryScoreValue = 10;
            }
            else // Ace (11/1)
            {
                newCard.cardPrimaryScoreValue = 11;
                newCard.cardSecondaryScoreValue = 1;
            }

            // Hide the face.
            newCard.Toggle(false, false);

            // Add the new card to the list.
            allCards.Add(newCard);
        }

        // Set the list of cards in deck equal to the whole cards list.
        cardsInDeck = allCards;

        // Reset Y order.
        SetCardsPositionByOrder();

        // Reset the first card reference.
        ResetFirstCard();

        SetManagingCards(false);
    }

    /// <summary>
    /// Click event.
    /// </summary>
    private void OnMouseUpAsButton_Event()
    {
        if (isManagingCards || GameManager.currentState != GameManager.TurnState.Game) return;
        if (cardsInDeck.Count <= 0) return;

        // If the operation hasn't been blocked, start the real objects management.
        StartCoroutine(OnMouseUpAsButton_Coroutine());
    }
    /// <summary>
    /// Manages the OnMouseUpAsButton event actions.
    /// </summary>
    /// <returns>IEnumerator value.</returns>
    private IEnumerator OnMouseUpAsButton_Coroutine()
    {
        SetManagingCards(true);
        deckHighlighter.SetActive(false);

        // Toggles the first card.
        firstCard.Toggle(true);

        yield return new WaitForSeconds(1);

        // The current player gets the first card, that is removed from the deck and marked as "Used".
        cardsInDeck.Remove(firstCard);
        PlayersManager.currentPlayer.GetCard(firstCard);

        yield return null;

        if (GameManager.currentState == GameManager.TurnState.Game) deckHighlighter.SetActive(true);
        SetManagingCards(false);
        yield return null;
    }

    /// <summary>
    /// MouseDown event.
    /// </summary>
    private void OnMouseDown_Event()
    {
        if (isManagingCards || GameManager.currentState != GameManager.TurnState.Game) return;
        if (cardsInDeck.Count <= 0) return;

        if (firstCard != null)
        {
            // If the operation hasn't been blocked, start the real objects management.
            SetManagingCards(true);
            deckHighlighter.SetActive(false);

            // Toggles the first card, that is removed from the deck and dragged.
            firstCard.Toggle(true);
            firstCard.IsDraggable = true;
            firstCard.DragStart();
            cardsInDeck.Remove(firstCard);

            ResetFirstCard();
        }
    }
    #endregion
    #endregion
}
