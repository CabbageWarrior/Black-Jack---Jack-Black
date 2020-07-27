using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace CabbageSoft.BlackJack.DeckManagement
{
    public class Deck : MonoBehaviour
    {
        #region Enums
        public enum ESuit
        {
            Hearts = 0,
            Diamonds = 1,
            Clubs = 2,
            Spades = 3
        }
        #endregion

        #region Inspector Infos
        /// <summary>
        /// Reference to the Card Prefab.
        /// </summary>
        public Card cardPrefab;
        /// <summary>
        /// Collection of sprites of the single cards.
        /// </summary>
        public Sprite[] cardFaces = default;
        /// <summary>
        /// Offset amount for cards in deck.
        /// </summary>
        public float cardOffset = 0f;

        /// <summary>
        /// Heatrs position when reordering.
        /// </summary>
        [Space]
        [Range(1, 4)]
        public int heartsSortPosition = 1;
        /// <summary>
        /// Diamonds position when reordering.
        /// </summary>
        [Range(1, 4)]
        public int diamondsSortPosition = 2;
        /// <summary>
        /// Clubs position when reordering.
        /// </summary>
        [Range(1, 4)]
        public int clubsSortPosition = 3;
        /// <summary>
        /// Spades position when reordering.
        /// </summary>
        [Range(1, 4)]
        public int spadesSortPosition = 4;

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

        #region Private Stuff
        /// <summary>
        /// List of all the cards.
        /// </summary>
        private List<Card> allCards = new List<Card>();
        /// <summary>
        /// List of all the cards currentlyin the deck.
        /// </summary>
        private List<Card> cardsInDeck;

        /// <summary>
        /// Is the deck managing cards right now?
        /// </summary>
        private bool isManagingCards = false;

        /// <summary>
        /// Sort order of suits when reorganizing cards.
        /// </summary>
        private List<ESuit> suitOrder = new List<ESuit>();
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

        #region UnityEvents
        /// <summary>
        /// Component Start method.
        /// </summary>
        private void Start()
        {
            // Initializing the sorting order.

            bool heartsSortManaged = false;
            bool diamondsSortManaged = false;
            bool clubsSortManaged = false;
            bool spadesSortManaged = false;

            while (!(heartsSortManaged && diamondsSortManaged && clubsSortManaged && spadesSortManaged))
            {
                int tmpMinSortNumber = 5;
                ESuit tmpMinSuit = ESuit.Hearts; // Only for initialization.

                // Finding the current minimum sorting value.
                if (!heartsSortManaged && heartsSortPosition < tmpMinSortNumber)
                {
                    tmpMinSortNumber = heartsSortPosition;
                    tmpMinSuit = ESuit.Hearts;
                }
                if (!diamondsSortManaged && diamondsSortPosition < tmpMinSortNumber)
                {
                    tmpMinSortNumber = diamondsSortPosition;
                    tmpMinSuit = ESuit.Diamonds;
                }
                if (!clubsSortManaged && clubsSortPosition < tmpMinSortNumber)
                {
                    tmpMinSortNumber = clubsSortPosition;
                    tmpMinSuit = ESuit.Clubs;
                }
                if (!spadesSortManaged && spadesSortPosition < tmpMinSortNumber)
                {
                    tmpMinSortNumber = spadesSortPosition;
                    tmpMinSuit = ESuit.Spades;
                }

                // Adding suit to the sorting list.
                suitOrder.Add(tmpMinSuit);

                // Checking which suit has been used.
                switch (tmpMinSuit)
                {
                    case ESuit.Hearts:
                        heartsSortManaged = true;
                        break;
                    case ESuit.Diamonds:
                        diamondsSortManaged = true;
                        break;
                    case ESuit.Clubs:
                        clubsSortManaged = true;
                        break;
                    case ESuit.Spades:
                        spadesSortManaged = true;
                        break;
                }
            }

            // Need to spawn the 52 cards.
            SpawnCards();

            // Reorder cards based on given suit order.
            ReorderCardsBySuit();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Shuffles the cards that are currently into the deck.
        /// </summary>
        public void ShuffleCardsInDeck()
        {
            if (isManagingCards || GameManager.currentState != GameManager.TurnState.Game) return;

            StartCoroutine(ShuffleCardsInDeck_Coroutine());

            IEnumerator ShuffleCardsInDeck_Coroutine()
            {
                SetManagingCards(true);
                deckHighlighter.SetActive(false);

                // For each card, the shuffle method swaps it with another random card that is part of the deck.
                int n = cardsInDeck.Count;
                int k;
                Transform cardTransform;
                Card temp;
                if (n > 0)
                {
                    while (n > 1)
                    {
                        n--;

                        cardTransform = cardsInDeck[n].transform;

                        // Make a "shuffle visual effect" moving cards on axis X and Z.
                        Sequence mySequenceX = DOTween.Sequence();
                        mySequenceX
                            .Append(cardTransform.DOMoveX(transform.position.x + Random.Range(-.2f, .2f), .2f))
                            .Append(cardTransform.DOMoveX(transform.position.x, .2f));
                        Sequence mySequenceZ = DOTween.Sequence();
                        mySequenceZ
                            .Append(cardTransform.DOMoveZ(transform.position.z + Random.Range(-.2f, .2f), .2f))
                            .Append(cardTransform.DOMoveZ(transform.position.z, .2f));

                        // Selecting the random index with which make the swap.
                        k = Random.Range(0, cardsInDeck.Count);

                        // Swapping cards.
                        temp = cardsInDeck[k];
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
        }

        /// <summary>
        /// Reorders the cards that are currently into the deck by their suit.
        /// </summary>
        public void ReorderCardsBySuit()
        {
            if (isManagingCards || GameManager.currentState != GameManager.TurnState.Game) return;

            StartCoroutine(ReorderCardsBySuit_Coroutine());

            IEnumerator ReorderCardsBySuit_Coroutine()
            {
                SetManagingCards(true);
                deckHighlighter.SetActive(false);

                int n = cardsInDeck.Count;
                List<Card> newDeckOrder;
                if (n > 0)
                {
                    newDeckOrder = new List<Card>();

                    for (int i = 0; i < suitOrder.Count; i++)
                    {
                        newDeckOrder.AddRange(cardsInDeck
                            .FindAll(x => x.CardSuit == suitOrder[i])
                            .OrderBy(x => x.CardSuitIndex)
                            .ToList()
                        );
                    }

                    cardsInDeck = newDeckOrder;

                    // Reorder the Y positions.
                    SetCardsPositionByOrder();

                    // Reset the first card reference after the shuffle.
                    ResetFirstCard();
                }

                if (GameManager.currentState == GameManager.TurnState.Game) deckHighlighter.SetActive(true);
                SetManagingCards(false);

                yield return null;
            }
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
                OnManagingCardsStart?.Invoke();
            }
            else
            {
                isManagingCards = false;
                OnManagingCardsEnd?.Invoke();
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
            return transform.position + Vector3.up * cardOffset * (cardsInDeck.Count + 1);
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
            firstCard = (cardsInDeck.Count > 0 ? cardsInDeck[0] : null);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Spawns the 52 cards and assigns their single values.
        /// </summary>
        private void SpawnCards()
        {
            SetManagingCards(true);

            // Cards List initialization.
            allCards.Clear();

            Card newCard;
            // Cards go from 2 to 11/1.
            for (int i = 0; i < 52; i++)
            {
                // Instantiates the card.
                newCard = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity);

                // Initializes its values.
                newCard.Initialize(i, (ESuit)Mathf.Floor(i / 13), i % 13, cardFaces[i], ResetFirstCard);

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
            if (PlayersManager.currentPlayer.CurrentState == Player.EState.Done) return;

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
            if (PlayersManager.currentPlayer.CurrentState == Player.EState.Done) yield return new WaitForSeconds(2f);

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
    }
}
