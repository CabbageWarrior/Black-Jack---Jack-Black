using CabbageSoft.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

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

    private int playerAINumber = default;
    
    private CharacterScriptableObject characterScriptableObject = default;
    private string playerAIName = default;
    private float riskPercentage = default;
    private int riskMinValue = default;
    
    private Sprite playerSprite = default;
    #endregion

    #region Properties
    public CharacterScriptableObject CharacterScriptableObject => characterScriptableObject;
    public string PlayerAIName => playerAIName;
    public float RiskPercentage => riskPercentage;
    public int RiskMinValue => riskMinValue;
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
        playerAINumber = dataManager.DefaultNumber;
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
        if (characterConfigIndex > dataManager.CharacterScriptableObjects.Count)
        {
            characterConfigIndex = dataManager.CharacterScriptableObjects.Count;
            return;
        }

        UpdatePlayerAIData();
    }

    private void UpdatePlayerAIData()
    {
        if (characterConfigIndex < dataManager.CharacterScriptableObjects.Count)
        {
            characterScriptableObject = dataManager.CharacterScriptableObjects[characterConfigIndex];

            playerAIName = characterScriptableObject.CharacterName;
            riskPercentage = characterScriptableObject.RiskPercentage;
            riskMinValue = characterScriptableObject.RiskCalcMinValue;
            playerSprite = characterScriptableObject.PortraitSprite;
            if (!playerSprite) playerSprite = dataManager.DefaultPortraitSprite; // fallback

            UpdateUI();

            ToggleUIActivation(false);
        }
        else
        {
            characterScriptableObject = null;

            ResetValues();

            ToggleUIActivation(true);
        }
    }
    private void ResetValues()
    {
        playerAIName = dataManager.DefaultName;
        riskPercentage = dataManager.DefaultRiskPercentage;
        riskMinValue = dataManager.DefaultRiskMinValue;
        playerSprite = dataManager.DefaultPortraitSprite;

        UpdateUI();
    }
    private void UpdateUI()
    {
        playerAINumberText.text = playerAINumber.ToString();
        playerAINameInput.text = playerAIName;
        riskPercentageSlider.value = riskPercentage;
        riskMinValueSlider.value = riskMinValue;
        playerImage.sprite = playerSprite;
    }
    private void ToggleUIActivation(bool active)
    {
        playerAINameInput.interactable = active;
        riskPercentageSlider.interactable = active;
        riskMinValueSlider.interactable = active;
    }
    #endregion
}
