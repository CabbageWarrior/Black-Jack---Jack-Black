using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerAI : Player
{
    #region Properties
    #region Public
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
    /// <summary>
    /// Reference to the DecisionCanvas.
    /// </summary>
    public Canvas decisionCanvas;
    /// <summary>
    /// Reference to the CurrentDecisionText.
    /// </summary>
    public Text currentDecisionText;

    /// <summary>
    /// Reference to the CharacterAnimator.
    /// </summary>
    public Animator characterAnimator;
    /// <summary>
    /// Reference to the CharacterFace.
    /// </summary>
    public GameObject characterFace;
    #endregion

    #region Private
    /// <summary>
    /// Reference to the coroutines that manage the DecisionPanel.
    /// </summary>
    private Coroutine manageDecisionPanelCoroutine;
    /// <summary>
    /// Initial scale of the CharacterFace.
    /// </summary>
    private Vector3 characterFaceInitialScale;
    #endregion
    #endregion

    #region Methods
    #region Public
    /// <summary>
    /// Sets the rotation of InfoCanvas.
    /// </summary>
    public void SetInfoCanvasRotation()
    {
        // The canvas must be in the same direction of the Main Camera.
        decisionCanvas.transform.rotation = Camera.main.transform.rotation;
    }

    /// <summary>
    /// Resets the player's infos and sends its cards back to the Deck.
    /// </summary>
    /// <returns>IEnumerator value.</returns>
    public override IEnumerator ResetInfos()
    {
        // Default ResetInfos functions...
        yield return StartCoroutine(base.ResetInfos());

        // Initialize the initial scale of the face.
        characterFaceInitialScale = characterFace.transform.localScale;

        HideDecisionPanelAfterSecs(0f);

        yield return null;
    }
    /// <summary>
    /// Sets the turn started for this player, invoking its events.
    /// </summary>
    public override void StartTurn()
    {
        // Default StartTurn functions...
        base.StartTurn();

        // Scales the face to a greater value in order to declare who is the current player.
        characterFace.transform.DOScale(characterFaceInitialScale + Vector3.one * .25f, .3f);

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
        if (!isBusted)
        {
            MakeDecision();
        }
        else
        {
            decisionCanvas.gameObject.SetActive(true);
            currentDecisionText.text = "Holy crap!";
            HideDecisionPanelAfterSecs(2f);
        }
    }
    /// <summary>
    /// Indicates that the player doesn't want more cards.
    /// </summary>
    public override void Stop()
    {
        characterAnimator.SetTrigger("Stop");

        decisionCanvas.gameObject.SetActive(true);
        currentDecisionText.text = "Ok, I'm done.";
        HideDecisionPanelAfterSecs(2f);

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
         * 3) The Dealer has busted.
         */

        // If the player is busted, it doesn't check the score.
        if (isBusted) return;

        if (isDealerBlackJack)
        {
            // If the dealer has a BlackJack, the player has lost the turn.
            SetLower();
        }
        else if (isBlackJack || currentScore > dealerScore || dealerScore > GameManager.BlackJackPoints)
        {
            SetHigher();
}
        else
        {
            SetLower();
        }
    }
    #endregion

    #region Protected
    /// <summary>
    /// Makes the player bust.
    /// </summary>
    protected override void Bust()
    {
        characterAnimator.SetTrigger("Busted");

        // Default Bust functions...
        base.Bust();
    }
    /// <summary>
    /// Ends the turn for this player, invoking its events.
    /// </summary>
    protected override void Finish()
    {
        // Resets the face's scale.
        characterFace.transform.DOScale(characterFaceInitialScale, .2f);

        // Default Finish functions...
        base.Finish();
    }
    #endregion

    #region Private
    /// <summary>
    /// Manages the player's decisions.
    /// </summary>
    private void MakeDecision()
    {
        // If has a BlackJack, the player stops.
        if (currentScore == GameManager.BlackJackPoints)
        {
            Stop();
            return;
        }

        // If has not reached the minimum score for risk calculation, the player asks one more card.
        if (currentScore < minValueForRiskCalculation)
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
        characterAnimator.SetTrigger("Ask Card");

        decisionCanvas.gameObject.SetActive(true);
        currentDecisionText.text = "Gimme my card number " + (currentCards.Count + 1).ToString() + "!";
    }
    /// <summary>
    /// Sets the player as a Winner.
    /// </summary>
    private void SetHigher()
    {
        characterAnimator.SetTrigger("Win");
        decisionCanvas.gameObject.SetActive(true);
        currentDecisionText.text = "YEEEEEAH!";
        HideDecisionPanelAfterSecs(2f);

        currentSituationText.text = "Win!";
    }
    /// <summary>
    /// Sets the player as a Loser.
    /// </summary>
    private void SetLower()
    {
        characterAnimator.SetTrigger("Lose");
        decisionCanvas.gameObject.SetActive(true);
        currentDecisionText.text = "Oh no!";
        HideDecisionPanelAfterSecs(2f);

        currentSituationText.text = "Lose...";
    }

    /// <summary>
    /// Hides the DecisionPanel after the seconds specified.
    /// </summary>
    /// <param name="secs">Seconds after which the panel disappears.</param>
    private void HideDecisionPanelAfterSecs(float secs)
    {
        // If a coroutine is already in progress, the system stops it in order to avoid strange behaviours.
        if (manageDecisionPanelCoroutine != null) StopCoroutine(manageDecisionPanelCoroutine);

        manageDecisionPanelCoroutine = StartCoroutine(HideDecisionPanelAfterSecs_Coroutine(secs));
    }
    /// <summary>
    /// Hides the DecisionPanel after the seconds specified.
    /// </summary>
    /// <param name="secs">Seconds after which the panel disappears.</param>
    /// <returns>IEnumerator value.</returns>
    private IEnumerator HideDecisionPanelAfterSecs_Coroutine(float secs)
    {
        yield return new WaitForSeconds(secs);

        decisionCanvas.gameObject.SetActive(false);
        currentDecisionText.text = "";

        yield return null;
    }
    #endregion
    #endregion
}
