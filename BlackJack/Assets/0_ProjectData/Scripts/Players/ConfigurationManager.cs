using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using CabbageSoft.BlackJack.Characters;

namespace CabbageSoft.BlackJack
{
    public class ConfigurationManager : MonoBehaviour
    {
        #region Properties
        #region Public
        public RectTransform rectTransformBlack = default;
        public RectTransform rectTransformJack = default;

        /// <summary>
        /// InputField reference to the Dealer's name.
        /// </summary>
        [Header("Phase 1")]
        public InputField dealerNameInput = default;
        /// <summary>
        /// Slider reference to the amountof AI players.
        /// </summary>
        [Space]
        public Slider playersAINumberSlider = default;
        [SerializeField] private Text playersAINumberMinText = default;
        [SerializeField] private Text playersAINumberMaxText = default;
        /// <summary>
        /// Dropdown reference to the card sending method.
        /// </summary>
        [Space]
        public Dropdown cardSendingMethodDropdown = default;

        /// <summary>
        /// Reference to all the PlayerAIConfigurators.
        /// </summary>
        [Header("Phase 2")]
        public PlayerAIConfigurator[] configurators = default;
        #endregion

        #region Private
        /// <summary>
        /// Reference to the DataManager undestroyable object.
        /// </summary>
        private DataManager dataManager = default;
        #endregion
        #endregion

        #region Events from Inspector
        /// <summary>
        /// Event to invoke when DataManager finishes its Load.
        /// </summary>
        [Space]
        public UnityEvent OnDataManagerLoadComplete = default;
        #endregion

        #region Methods
        #region MonoBehaviour Methods
        /// <summary>
        /// Component Start Method.
        /// </summary>
        /// <returns>IEnumerator value.</returns>
        private IEnumerator Start()
        {
            // Waiting to find the DataManager reference. DataReference is a "DontDestroyOnLoad" object from the Menu scene.
            while (dataManager == null)
            {
                dataManager = GameObject.FindObjectOfType<DataManager>();
            }

            // Fixed configuration
            playersAINumberSlider.minValue = dataManager.ConfigurationData.PlayersNumberMin;
            playersAINumberSlider.maxValue = dataManager.ConfigurationData.PlayersNumberMax;

            playersAINumberMinText.text = dataManager.ConfigurationData.PlayersNumberMin.ToString();
            playersAINumberMaxText.text = dataManager.ConfigurationData.PlayersNumberMax.ToString();

            if (dataManager.ConfigurationData.DefaultPlayersNumber < playersAINumberSlider.minValue) playersAINumberSlider.value = playersAINumberSlider.minValue;
            else if (dataManager.ConfigurationData.DefaultPlayersNumber > playersAINumberSlider.maxValue) playersAINumberSlider.value = playersAINumberSlider.maxValue;
            else playersAINumberSlider.value = dataManager.ConfigurationData.DefaultPlayersNumber;

            cardSendingMethodDropdown.value = (int)dataManager.ConfigurationData.DefaultCardSendingMethod;

            if (OnDataManagerLoadComplete != null) OnDataManagerLoadComplete.Invoke();
            yield return null;
        }
        #endregion

        #region Public
        /// <summary>
        /// Moves the two texts.
        /// </summary>
        public void MoveBlackAndJack()
        {
            StartCoroutine(MoveBlackAndJack_Coroutine());
        }
        private IEnumerator MoveBlackAndJack_Coroutine()
        {
            yield return new WaitForSeconds(1f);

            Transform blackFinalTransform = rectTransformJack.transform;
            Transform jackFinalTransform = rectTransformBlack.transform;

            rectTransformBlack.DOMoveX(blackFinalTransform.position.x, 2f);
            rectTransformBlack.DORotateQuaternion(blackFinalTransform.rotation, 2f);

            rectTransformJack.DOMoveX(jackFinalTransform.position.x, 2f);
            rectTransformJack.DORotateQuaternion(jackFinalTransform.rotation, 2f);

            yield return null;
        }

        /// <summary>
        /// Configures informations used to initialize the Phase 2.
        /// </summary>
        public void ConfigPhaseAI()
        {
            dataManager.ClearPlayersToInit();

            dataManager.DealerName = (!string.IsNullOrEmpty(dealerNameInput.text) ? dealerNameInput.text : "Missingno");
            dataManager.CardSendingMode = (DataManager.CardSendingType)cardSendingMethodDropdown.value;

            // Resets informations in players' configurators and activates only the amount required in Phase 1.
            for (int i = 0; i < configurators.Length; i++)
            {
                configurators[i].DeactivateConfigurator();

                if (i < playersAINumberSlider.value)
                {
                    configurators[i].ActivateConfigurator(i);
                }
            }
        }

        /// <summary>
        /// Sends data to the DataManager.
        /// </summary>
        public void CommitData()
        {
            // For each active configurator block, the system creates a data block that is added to the undestroyable object in order to be used in the Game scene.
            for (int i = 0; i < playersAINumberSlider.value; i++)
            {
                PlayerAIConfigurator currentConfig = configurators[i];

                // Data Validation
                if (string.IsNullOrEmpty(currentConfig.CharacterAIProperties.CharacterName))
                {
                    currentConfig.CharacterAIProperties.CharacterName = string.Concat("Cyborg#", (i + 1).ToString("D2"));
                }

                DataManager.PlayerAIInitData data = new DataManager.PlayerAIInitData
                {
                    CharacterAIScriptableObject = currentConfig.CharacterAIScriptableObject,
                    CharacterAIProperties = currentConfig.CharacterAIProperties
                };

                dataManager.AddPlayerAI(data);
            }

            // After that, the system loads the Game scene.
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }

        /// <summary>
        /// Exits.
        /// </summary>
        public void ExitGame()
        {
            Application.Quit();
        }
        #endregion
        #endregion
    }
}
