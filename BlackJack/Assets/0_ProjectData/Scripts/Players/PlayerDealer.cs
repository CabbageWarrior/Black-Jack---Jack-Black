using UnityEngine;
using DG.Tweening;

public class PlayerDealer : Player
{
    #region Properties
    #region Public
    /// <summary>
    /// Reference to PlayersManager.
    /// </summary>
    public PlayersManager playersManager;
    #endregion
    #endregion

    #region Methods
    #region Public
    /// <summary>
    /// True if the Dealer has a BlackJack, False otherwise.
    /// </summary>
    public bool IsBlackJack
    {
        get {
            return isBlackJack;
        }
    }
    #endregion

    #region Protected
    /// <summary>
    /// Sets the new position to the card.
    /// </summary>
    /// <param name="card">Card to move.</param>
    protected override void SetCardPosition(Card card)
    {
        // Moves the previous cards on the X axis in order to free some space for the new card.
        foreach (Card prevCard in currentCards)
        {
            if (!prevCard.Equals(card))
            {
                prevCard.transform.DOMoveX(prevCard.transform.position.x - .07f, .2f);
            }
        }

        card.transform.DOMove(cardSlot.transform.position + new Vector3(0f, .001f * currentCards.Count, 0f), .5f);
    }
    #endregion
    #endregion
}
