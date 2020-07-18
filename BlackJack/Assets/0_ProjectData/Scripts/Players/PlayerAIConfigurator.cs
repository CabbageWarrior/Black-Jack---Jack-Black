using UnityEngine;
using UnityEngine.UI;
using CabbageSoft.JackBlack.ScriptableObjects;
using CabbageSoft.JackBlack.Properties;

public class PlayerAIConfigurator : MonoBehaviour
{
    #region Inspector Infos
    [SerializeField] private DataManager dataManager = default;
    [Space]
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
    [Space]
    [SerializeField] private Image playerImage = default;
    #endregion

    #region Private Stuff
    private int characterConfigIndex = 0;

    private int playerAINumber = 0;
    #endregion

    #region Properties
    public CharacterAIScriptableObject CharacterAIScriptableObject { get; private set; } = default;
    public CharacterAIProperties CharacterAIProperties { get; private set; } = default;
    #endregion

    #region Methods
    /// <summary>
    /// Activates the Configurator panel and adds the correct number in it.
    /// </summary>
    /// <param name="configIndex"></param>
    public void ActivateConfigurator(int configIndex)
    {
        playerAINumber = (configIndex + 1);

        UpdatePlayerAIData();

        panelInfos.SetActive(true);
    }

    /// <summary>
    /// Resets the COnfigurator infos and deactivates it.
    /// </summary>
    public void DeactivateConfigurator()
    {
        playerAINumber = 0;
        ResetValues();

        panelInfos.SetActive(false);
    }
    public void CharPrev()
    {
        characterConfigIndex--;
        if (characterConfigIndex < 0)
        {
            characterConfigIndex = 0;
            return;
        }

        UpdatePlayerAIData();
    }
    public void CharNext()
    {
        characterConfigIndex++;
        if (characterConfigIndex > dataManager.ConfigurationData.CharacterScriptableObjects.Count)
        {
            characterConfigIndex = dataManager.ConfigurationData.CharacterScriptableObjects.Count;
            return;
        }

        UpdatePlayerAIData();
    }

    private void UpdatePlayerAIData()
    {
        if (characterConfigIndex < dataManager.ConfigurationData.CharacterScriptableObjects.Count)
        {
            CharacterAIScriptableObject = dataManager.ConfigurationData.CharacterScriptableObjects[characterConfigIndex];

            CharacterAIProperties = CharacterAIScriptableObject.Properties.Clone();
            if (!CharacterAIProperties.PortraitSprite) CharacterAIProperties.PortraitSprite = dataManager.ConfigurationData.DefaultCharacterAIProperties.PortraitSprite; // fallback

            UpdateUI();

            ToggleUIActivation(false);
        }
        else
        {
            CharacterAIScriptableObject = null;

            ResetValues();

            ToggleUIActivation(true);
        }
    }
    private void ResetValues()
    {
        CharacterAIProperties = dataManager.ConfigurationData.DefaultCharacterAIProperties.Clone();

        UpdateUI();
    }
    private void UpdateUI()
    {
        playerAINumberText.text = playerAINumber.ToString();
        playerAINameInput.text = CharacterAIProperties.CharacterName;
        riskPercentageSlider.value = CharacterAIProperties.RiskPercentage;
        riskMinValueSlider.value = CharacterAIProperties.RiskCalcMinScore;
        playerImage.sprite = CharacterAIProperties.PortraitSprite;
    }
    private void ToggleUIActivation(bool active)
    {
        playerAINameInput.interactable = active;
        riskPercentageSlider.interactable = active;
        riskMinValueSlider.interactable = active;
    }
    #endregion
}
