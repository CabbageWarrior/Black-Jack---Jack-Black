using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAIMatchInfoController : MonoBehaviour
{

    /// <summary>
    /// Reference to the DecisionCanvas.
    /// </summary>
    public GameObject decisionCanvas = default;
    /// <summary>
    /// Reference to the CurrentDecisionText.
    /// </summary>
    public Text currentDecisionText = default;

    /// <summary>
    /// Reference to the Text Component of the name.
    /// </summary>
    [Header("Components")]
    [SerializeField] private Text nameText = default;
    /// <summary>
    /// Reference to the Text Component of the score.
    /// </summary>
    [SerializeField] private Text currentScoreText = default;
    /// <summary>
    /// Reference to the Text Component of the current situation.
    /// </summary>
    [SerializeField] private Text currentSituationText = default;


    /// <summary>
    /// Reference to the coroutines that manage the DecisionPanel.
    /// </summary>
    private Coroutine manageDecisionPanelCoroutine = default;

    public Text NameText => nameText;
    public Text CurrentScoreText => currentScoreText;
    public Text CurrentSituationText => currentSituationText;


    public void ShowDialogue(string text, float showTime)
    {
        decisionCanvas.SetActive(true);
        currentDecisionText.text = text;
        if (showTime >= 0f)
        {
            HideDecisionPanelAfterSecs(showTime);
        }
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

        IEnumerator HideDecisionPanelAfterSecs_Coroutine(float p_secs)
        {
            yield return new WaitForSeconds(p_secs);

            decisionCanvas.SetActive(false);
            currentDecisionText.text = string.Empty;

            yield return null;
        }
    }
}
