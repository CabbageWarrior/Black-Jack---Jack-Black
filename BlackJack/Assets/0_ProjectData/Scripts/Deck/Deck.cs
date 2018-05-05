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
    private List<Card> cards;
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
                transform.position.y - cardOffset * (i + cards.Count - cardsInDeck.Count),
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

            Vector3 deckInitialPosition = transform.position;
            Vector3 newPositionPoint = deckInitialPosition + new Vector3(0f, cardOffset * cards.Count, 0f);

            // For each card, reset its state and reinsert it into the deck.
            foreach (Card card in cards)
            {
                card.ResetState();
                card.transform.DOMove(newPositionPoint, .1f);
                cardsInDeck.Add(card);

                yield return new WaitForSeconds(.05f);
            }
            // Clear the player's cards list.
            cards.Clear();

            SetManagingCards(false);
        }

        yield return null;
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
        if (cards == null)
        {
            cards = new List<Card>();
        }
        else
        {
            cards.Clear();
        }

        // Cards go from 2 to 11/1.
        for (int i = 0; i < 52; i++)
        {
            GameObject newCardGO = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);

            Card newCard = newCardGO.GetComponent<Card>();
            newCard.cardIndex = i;

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

            // Hide the face and initialize the callback
            newCard.Toggle(false, false);
            newCard.OnUsed += ResetFirstCard;

            // Add the new card to the list.
            cards.Add(newCard);
        }

        // Set the list of cards in deck equal to the whole cards list.
        cardsInDeck = cards;

        // Reset Y order.
        SetCardsPositionByOrder();

        // Reset the first card reference.
        ResetFirstCard();

        SetManagingCards(false);
    }

    /// <summary>
    /// Overwrites the first card reference with the current first card of the deck.
    /// </summary>
    private void ResetFirstCard()
    {
        firstCard = cardsInDeck[0];
    }

    /// <summary>
    /// Changes the card management state.
    /// </summary>
    /// <param name="isManaging">True if the deck is starting the management, False otherwise.</param>
    private void SetManagingCards(bool isManaging)
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

    /// <summary>
    /// Manages the Click event actions.
    /// </summary>
    /// <returns>IEnumerator value.</returns>
    private IEnumerator DeckClicked_Coroutine()
    {
        SetManagingCards(true);
        deckHighlighter.SetActive(false);

        // Toggles the first card.
        firstCard.Toggle(true);

        yield return new WaitForSeconds(1);

        // The current player gets the first card, that is removed from the deck and marked as "Used".
        PlayersManager.currentPlayer.GetCard(firstCard);
        cardsInDeck.Remove(firstCard);
        firstCard.Use();

        yield return null;

        if (GameManager.currentState == GameManager.TurnState.Game) deckHighlighter.SetActive(true);
        SetManagingCards(false);
        yield return null;
    }
    #endregion
    #endregion

    #region Interaction Events
    /// <summary>
    /// Click event.
    /// </summary>
    private void OnMouseUpAsButton()
    {
        if (isManagingCards || GameManager.currentState != GameManager.TurnState.Game) return;
        if (cardsInDeck.Count <= 0) return;

        // If the operation hasn't been blocked, start the real objects management.
        StartCoroutine(DeckClicked_Coroutine());
    }
    #endregion
}
