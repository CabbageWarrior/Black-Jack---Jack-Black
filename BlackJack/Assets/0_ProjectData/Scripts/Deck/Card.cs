using UnityEngine;
using DG.Tweening;

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
    #endregion

    #region Private
    /// <summary>
    /// SpriteRenderer reference used for setting the correct Sprite.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Used to know if the card has been used.
    /// </summary>
    private bool isUsed = false;
    /// <summary>
    /// Is this card already used?
    /// </summary>
    public bool IsUsed
    {
        get
        {
            return isUsed;
        }
    }
    #endregion
    #endregion

    #region Methods
    #region MonoBehaviour Methods
    /// <summary>
    /// Component Awake method.
    /// </summary>
    private void Awake()
    {
        // Initializes the SpriteRenderer reference.
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Component Start method.
    /// </summary>
    private void Start()
    {
        // Sets the correct face's Sprite based on the Card Index.
        SetCardFace();
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
            mySequence
                .Append(transform.DOLocalMoveY(startingPositionY + .1f, toggleDuration / 2f))
                .AppendInterval(toggleDuration / 2f)
                .Append(transform.DOLocalMoveY(startingPositionY, toggleDuration / 2f))
                .Insert(0, transform.DOLocalRotateQuaternion(newAngle, mySequence.Duration()));
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
        isUsed = true;
        if (OnUsed != null) OnUsed();
    }

    /// <summary>
    /// Resets the state of the card as unused and with the face hidden.
    /// </summary>
    public void ResetState()
    {
        isUsed = false;
        Toggle(false);
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
    #endregion
    #endregion
}
