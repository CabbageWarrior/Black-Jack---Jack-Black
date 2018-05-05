using UnityEngine;
using UnityEngine.UI;

public class PlayerAIConfigurator : MonoBehaviour
{
    #region Consts
    /// <summary>
    /// Default Player Index.
    /// </summary>
    const int defaultNumber = 0;
    /// <summary>
    /// Default Player Name.
    /// </summary>
    const string defaultName = "";
    /// <summary>
    /// Default Player Risk Percentage.
    /// </summary>
    const float defaultRiskPercentage = 50f;
    /// <summary>
    /// Default Player Minimum Score after which calculate risk.
    /// </summary>
    const int defaultRiskMinValue = 10;
    #endregion

    #region Properties
    #region Public
    /// <summary>
    /// PanelInfos reference.
    /// </summary>
    public GameObject panelInfos;
    /// <summary>
    /// PlayerAINumberText reference.
    /// </summary>
    [Space]
    public Text playerAINumberText;
    /// <summary>
    /// PlayerAINameInput reference.
    /// </summary>
    public InputField playerAINameInput;
    /// <summary>
    /// PlayerAIRiskPercentageSlider reference.
    /// </summary>
    public Slider riskPercentageSlider;
    /// <summary>
    /// PlayerAIRiskMinValueSlider reference.
    /// </summary>
    public Slider riskMinValueSlider;
    #endregion
    #endregion

    #region Methods
    #region Public
    /// <summary>
    /// Resets the COnfigurator infos and deactivates it.
    /// </summary>
    public void ResetValues()
    {
        playerAINumberText.text = defaultNumber.ToString();
        playerAINameInput.text = defaultName;
        riskPercentageSlider.value = defaultRiskPercentage;
        riskMinValueSlider.value = defaultRiskMinValue;

        panelInfos.SetActive(false);
    }

    /// <summary>
    /// Activates the Configurator panel and adds the correct number in it.
    /// </summary>
    /// <param name="configIndex"></param>
    public void ActivateConfigurator(int configIndex)
    {
        playerAINumberText.text = (configIndex + 1).ToString();

        panelInfos.SetActive(true);
    }
    #endregion
    #endregion
}
