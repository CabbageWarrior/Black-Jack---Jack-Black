using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System;

namespace CabbageSoft.BlackJack.DeckManagement
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Card : MonoBehaviour
    {
        #region Inspector Infos
        /// <summary>
        /// What happens when the card is used.
        /// </summary>
        public UnityEvent OnUsed = default;
        #endregion

        #region Private Stuff
        /// <summary>
        /// SpriteRenderer reference used for setting the correct Sprite.
        /// </summary>
        private SpriteRenderer spriteRenderer;
        /// <summary>
        /// Rigidbody reference used for velocity.
        /// </summary>
        private Rigidbody cardRigidbody;

        private Coroutine flightCoroutine;

        private Deck deck = default;

        private Quaternion cardShownQuaternion = Quaternion.Euler(90, 180, 180);
        private Quaternion cardHiddenQuaternion = Quaternion.Euler(270, 0, 180);
        #endregion

        #region Properties
        /// <summary>
        /// Is this card draggable?
        /// </summary>
        public bool IsDraggable { get; set; } = false;
        /// <summary>
        /// Is this card being dragged?
        /// </summary>
        public bool IsDragging { get; private set; } = false;
        /// <summary>
        /// Is this card already used?
        /// </summary>
        public bool IsUsed { get; private set; } = false;

        /// <summary>
        /// Card index.
        /// </summary>
        public int CardIndex { get; private set; } = default;
        /// <summary>
        /// Suit of the card.
        /// </summary>
        public Deck.ESuit CardSuit { get; private set; } = default;
        /// <summary>
        /// Card suit index.
        /// </summary>
        public int CardSuitIndex { get; private set; } = default;
        /// <summary>
        /// Primary score value of the card.
        /// </summary>
        public int CardPrimaryScoreValue { get; private set; } = default;
        /// <summary>
        /// Secondary score value of the card, used in case of Soft hands.
        /// </summary>
        public int CardSecondaryScoreValue { get; private set; } = default;
        #endregion

        #region UnityEvents
        /// <inheritdoc/>
        private void Update()
        {
            if (!GameManager.instance) return;
            if (!GameManager.instance.DataManager) return;
            if (IsUsed) return;

            if (GameManager.instance.DataManager.CardSendingMode == DataManager.CardSendingType.DragAndDrop)
            {
                if (IsDragging && Input.GetMouseButtonUp(0))
                {
                    DragStop();
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes the values of the new card.
        /// </summary>
        /// <param name="p_cardIndex">Card index (0 to 51).</param>
        /// <param name="p_cardSuit">Card suit (0 to 3).</param>
        /// <param name="p_cardSuitIndex">Card suit index (0 to 12).</param>
        /// <param name="p_OnUsedCallback">Callback called on card use.</param>
        public void Initialize(
            Deck p_deck,
            int p_cardIndex,
            Deck.ESuit p_cardSuit,
            int p_cardSuitIndex,
            Sprite cardFaceSprite,
            UnityAction p_OnUsedCallback
        )
        {
            // Initializes the SpriteRenderer and Rigidbody references.
            spriteRenderer = GetComponent<SpriteRenderer>();
            cardRigidbody = GetComponent<Rigidbody>();

            // Parameters
            deck = p_deck;
            CardIndex = p_cardIndex;
            CardSuit = p_cardSuit;
            CardSuitIndex = p_cardSuitIndex;

            spriteRenderer.sprite = cardFaceSprite;

            OnUsed.AddListener(p_OnUsedCallback);

            // Initializing card values.
            if (CardSuitIndex < 9) // Numbers 2-10
            {
                CardPrimaryScoreValue = CardSuitIndex + 2;
                CardSecondaryScoreValue = CardSuitIndex + 2;
            }
            else if (CardSuitIndex < 12) // Figures
            {
                CardPrimaryScoreValue = 10;
                CardSecondaryScoreValue = 10;
            }
            else // Ace (11/1)
            {
                CardPrimaryScoreValue = 11;
                CardSecondaryScoreValue = 1;
            }
        }

        /// <summary>
        /// Toggles the card's face.
        /// </summary>
        /// <param name="showCard">True if you want to show the card's face, False if you want to hide it.</param>
        /// <param name="useAnimation">True if you want to use the animation when toggling, otherwise False. Default: True.</param>
        public void Toggle(bool showCard, bool useAnimation = true)
        {
            // Declaring temporary variables.
            float toggleDuration;
            Quaternion newAngle;

            // Set destination values based on toggle mode.
            if (showCard)
            {
                toggleDuration = .5f;
                newAngle = cardShownQuaternion;
            }
            else
            {
                toggleDuration = .1f;
                newAngle = cardHiddenQuaternion;
            }

            // Set new card position with or without animation.
            if (useAnimation)
            {
                float startingPositionY = transform.localPosition.y;

                Sequence mySequence = DOTween.Sequence();
                if (GameManager.instance.DataManager.CardSendingMode == DataManager.CardSendingType.SingleClick)
                {
                    mySequence
                        .Append(transform.DOLocalMoveY(startingPositionY + .1f, toggleDuration / 2f))
                        .AppendInterval(toggleDuration / 2f)
                        .Append(transform.DOLocalMoveY(startingPositionY, toggleDuration / 2f))
                        .Insert(0, transform.DOLocalRotateQuaternion(newAngle, mySequence.Duration()));
                }
                else
                {
                    mySequence
                        .Append(transform.DOLocalRotateQuaternion(newAngle, toggleDuration));
                }
            }
            else
            {
                transform.rotation = newAngle;
            }
        }

        /// <summary>
        /// Sets the Card as Used.
        /// </summary>
        public void Use()
        {
            IsUsed = true;
            IsDraggable = false;
            FreezePosition();
            if (flightCoroutine != null)
            {
                StopCoroutine(flightCoroutine);
            }
            OnUsed?.Invoke();
        }

        /// <summary>
        /// Resets the state of the card as unused and with the face hidden.
        /// </summary>
        public void ResetState()
        {
            tag = "Card";
            FreezePosition();
            IsUsed = false;
            IsDraggable = false;
            IsDragging = false;
            Toggle(false);
        }
        /// <summary>
        /// Interrupts the physics movement.
        /// </summary>
        public void FreezePosition()
        {
            cardRigidbody.velocity = Vector3.zero;
            cardRigidbody.angularVelocity = Vector3.zero;
        }

        public void DragStart()
        {
            if (IsDraggable && !IsDragging)
            {
                StartCoroutine(DragMovement_Coroutine());
            }

            IEnumerator DragMovement_Coroutine()
            {
                if (flightCoroutine != null)
                {
                    StopCoroutine(flightCoroutine);
                }

                IsDragging = true;
                tag = "ValidCard";

                FreezePosition();

                Ray rayFromCamera;
                RaycastHit hitFromCamera;

                int layer9 = 1 << 9;

                Vector3 prevPosition = transform.position, actualPosition = transform.position;

                while (IsDragging)
                {
                    rayFromCamera = Camera.main.ScreenPointToRay(Input.mousePosition);

                    Debug.DrawRay(rayFromCamera.origin, rayFromCamera.direction * 10, Color.red);

                    if (Physics.Raycast(rayFromCamera, out hitFromCamera, 100, layer9))
                    {
                        prevPosition = actualPosition;

                        transform.position = hitFromCamera.point;
                        actualPosition = transform.position;
                    }
                    yield return null;
                }

                // Adding velocity
                Vector3 lastFrameVector = actualPosition - prevPosition;
                cardRigidbody.AddForce(lastFrameVector * 10, ForceMode.Impulse);

                if (flightCoroutine != null)
                {
                    StopCoroutine(flightCoroutine);
                }
                flightCoroutine = StartCoroutine(CardReturnCountdown_Coroutine());

                yield return null;

                IEnumerator CardReturnCountdown_Coroutine()
                {
                    yield return new WaitForSeconds(3f);

                    ReturnIntoDeck();
                }
            }
        }
        public void DragStop()
        {
            IsDragging = false;
            if (GameManager.currentState == GameManager.TurnState.Game) deck.DeckHighlighter.SetActive(true);
            deck.SetManagingCards(false);
        }

        /// <summary>
        /// Triggers the click event on the card.
        /// </summary>
        public void ClickEvent()
        {
            if (GameManager.instance.DataManager.CardSendingMode == DataManager.CardSendingType.DragAndDrop)
            {
                if (!IsUsed)
                {
                    DragStart();
                }
            }
        }

        public void ReturnIntoDeck()
        {
            ResetState();
            transform
                .DOMove(deck.ReaddCardPosition(), .3f)
                .OnComplete(ReturnIntoDeck_Callback);

            void ReturnIntoDeck_Callback()
            {
                deck.AddCard(this);
                deck.SetCardsPositionByOrder();
                deck.ResetFirstCard();
            }
        }
        #endregion
    }
}