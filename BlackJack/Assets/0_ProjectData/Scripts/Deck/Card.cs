using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace CabbageSoft.BlackJack
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Card : MonoBehaviour
    {
        #region Delegate Types
        /// <summary>
        /// OnUsed Delegate Type.
        /// </summary>
        public delegate void OnUsedDelegate();
        #endregion

        #region Properties
        #region Public
        /// <summary>
        /// Collection of sprites of the single cards.
        /// </summary>
        public Sprite[] faces;

        /// <summary>
        /// Card index.
        /// </summary>
        [Space]
        public int cardIndex;
        /// <summary>
        /// Suit of the card.
        /// </summary>
        public Deck.ESuit cardSuit;
        /// <summary>
        /// Card suit index.
        /// </summary>
        public int cardSuitIndex;
        /// <summary>
        /// Primary score value of the card.
        /// </summary>
        public int cardPrimaryScoreValue = 0;
        /// <summary>
        /// Secondary score value of the card, used in case of Soft hands.
        /// </summary>
        public int cardSecondaryScoreValue = 0;

        /// <summary>
        /// What happens when the card is used.
        /// </summary>
        public OnUsedDelegate OnUsed = null;

        /// <summary>
        /// Is this card draggable?
        /// </summary>
        public bool IsDraggable { get; set; } = false;

        public bool IsDragging { get; private set; } = false;
        #endregion

        #region Private
        /// <summary>
        /// SpriteRenderer reference used for setting the correct Sprite.
        /// </summary>
        private SpriteRenderer spriteRenderer;
        /// <summary>
        /// Rigidbody reference used for velocity.
        /// </summary>
        private Rigidbody cardRigidbody;

        /// <summary>
        /// Is this card already used?
        /// </summary>
        public bool IsUsed { get; private set; } = false;

        private Coroutine flightCoroutine;
        #endregion
        #endregion

        #region Methods
        #region MonoBehaviour Methods
        /// <summary>
        /// Component Awake method.
        /// </summary>
        private void Awake()
        {
            // Initializes the SpriteRenderer and Rigidbody references.
            spriteRenderer = GetComponent<SpriteRenderer>();
            cardRigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Component Start method.
        /// </summary>
        private void Start()
        {
            // Sets the correct face's Sprite based on the Card Index.
            SetCardFace();
        }

        /// <summary>
        /// Component Update method.
        /// </summary>
        private void Update()
        {
            if (!IsUsed && GameManager.instance != null && GameManager.instance.DataManager != null && GameManager.instance.DataManager.CardSendingMode == DataManager.CardSendingType.DragAndDrop)
            {
                if (IsDragging && Input.GetMouseButtonUp(0))
                {
                    DragStop();
                }
            }
        }
        #endregion

        #region Public
        /// <summary>
        /// Toggles the card's face.
        /// </summary>
        /// <param name="showCard">True if you want to show the card's face, False if you want to hide it.</param>
        /// <param name="useAnimation">True if you want to use the animation when toggling, otherwise False.</param>
        public void Toggle(bool showCard, bool? useAnimation = true)
        {
            float toggleDuration;
            Quaternion newAngle;

            if (showCard)
            {
                toggleDuration = .5f;
                newAngle = Quaternion.Euler(90, 180, 180);
            }
            else
            {
                toggleDuration = .1f;
                newAngle = Quaternion.Euler(270, 0, 180);
            }

            if (useAnimation == true)
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
        }
        public void DragStop()
        {
            IsDragging = false;
            if (GameManager.currentState == GameManager.TurnState.Game) GameManager.StaticDeckRef.deckHighlighter.SetActive(true);
            GameManager.StaticDeckRef.SetManagingCards(false);
        }

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
            transform.DOMove(GameManager.StaticDeckRef.ReaddCardPosition(), .3f).OnComplete(ReturnIntoDeck_Callback);
        }
        private void ReturnIntoDeck_Callback()
        {
            GameManager.StaticDeckRef.AddCard(this);
            GameManager.StaticDeckRef.SetCardsPositionByOrder();
            GameManager.StaticDeckRef.ResetFirstCard();
        }
        #endregion

        #region Private
        /// <summary>
        /// Sets the correct Sprite to the face of the card, according to its Index.
        /// </summary>
        private void SetCardFace()
        {
            spriteRenderer.sprite = faces[cardIndex];
        }

        private IEnumerator DragMovement_Coroutine()
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
        }

        private IEnumerator CardReturnCountdown_Coroutine()
        {
            yield return new WaitForSeconds(3f);

            ReturnIntoDeck();
        }
        #endregion
        #endregion
    }
}