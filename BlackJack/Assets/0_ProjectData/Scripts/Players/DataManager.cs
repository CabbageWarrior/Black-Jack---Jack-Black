using System.Collections.Generic;
using UnityEngine;
using CabbageSoft.BlackJack.ScriptableObjects;
using CabbageSoft.BlackJack.Properties;

namespace CabbageSoft.BlackJack
{
    public class DataManager : MonoBehaviour
    {
        #region Enums
        /// <summary>
        /// Type of method used to send cards to the players.
        /// </summary>
        public enum CardSendingType
        {
            SingleClick = 0,
            DragAndDrop = 1
        }
        #endregion

        #region Structs
        /// <summary>
        /// Data used to initialize AI Players.
        /// </summary>
        public struct PlayerAIInitData
        {
            public CharacterAIScriptableObject CharacterAIScriptableObject;

            public CharacterAIProperties CharacterAIProperties;
        }
        #endregion

        #region Statics
        /// <summary>
        /// Used to know if the DataManager has been created.
        /// </summary>
        private static bool created = false;
        #endregion

        #region Inspector Infos
        /// <summary>
        /// Configuration Data.
        /// </summary>
        [Header("Default Configuration")]
        [SerializeField] private ConfigurationDataScriptableObject configurationData = default;
        #endregion

        #region Private Stuff
        /// <summary>
        /// List used to register informations about the players that the system must initialize in game.
        /// </summary>
        private List<PlayerAIInitData> playersToInit = new List<PlayerAIInitData>();
        #endregion

        #region Properties
        public ConfigurationDataScriptableObject ConfigurationData => configurationData;
        /// <summary>
        /// The dealer's name.
        /// </summary>
        public string DealerName { get; set; }
        /// <summary>
        /// The mode in which cards are sent to players.
        /// </summary>
        public CardSendingType CardSendingMode { get; set; }
        #endregion

        #region Methods
        #region MonoBehaviour Methods
        /// <summary>
        /// Component Awake method.
        /// </summary>
        private void Awake()
        {
            // If the object is not undestroyable, the system sets it as undestroyable.
            if (!created)
            {
                DontDestroyOnLoad(this.gameObject);
                created = true;
            }
        }
        #endregion

        #region Public
        /// <summary>
        /// Clears the list of data.
        /// </summary>
        public void ClearPlayersToInit()
        {
            playersToInit.Clear();
        }

        /// <summary>
        /// Adds data to the list of players.
        /// </summary>
        public void AddPlayerAI(PlayerAIInitData data)
        {
            playersToInit.Add(data);
        }

        /// <summary>
        /// Gets the list of players.
        /// </summary>
        public List<PlayerAIInitData> GetPlayersToInit()
        {
            return playersToInit;
        }
        #endregion
        #endregion
    }
}
