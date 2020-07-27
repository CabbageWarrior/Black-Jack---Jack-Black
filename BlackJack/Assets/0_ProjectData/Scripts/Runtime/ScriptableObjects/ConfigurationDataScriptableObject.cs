using System.Collections.Generic;
using UnityEngine;
using CabbageSoft.BlackJack.Properties;

namespace CabbageSoft.BlackJack.ScriptableObjects
{
    /// <summary>
    /// <para>Class that contains the configuration of the application.</para>
    /// </summary>
    [CreateAssetMenu(fileName = "ConfigurationData", menuName = "CabbageSoft/ScriptableObjects/ConfigurationData", order = 0)]
    public class ConfigurationDataScriptableObject : ScriptableObject
    {
        #region Inspector Infos
        /// <summary>
        /// Default human Character infos.
        /// </summary>
        [Header("Default Configuration", order = 0)]
        [Header("==> Human Character", order = 1)]
        [SerializeField] private CharacterProperties defaultCharacterProperties = default;
        /// <summary>
        /// Default Character AI infos.
        /// </summary>
        [Header("==> Characters AI")]
        [SerializeField] private CharacterAIProperties defaultCharacterAIproperties = default;
        /// <summary>
        /// List of preconfigured Characters AI.
        /// </summary>
        [Space]
        [SerializeField] private List<CharacterAIScriptableObject> characterScriptableObjects = new List<CharacterAIScriptableObject>();

        [Header("==> Match")]
        [SerializeField] private int playersNumberMin = default;
        [SerializeField] private int playersNumberMax = default;
        [SerializeField] private int defaultPlayersNumber = default;
        [Space]
        [SerializeField] private DataManager.CardSendingType defaultCardSendingMethod = default;
        #endregion

        #region Properties
        /// <summary>
        /// Default human Character infos.
        /// </summary>
        public CharacterProperties DefaultCharacterProperties => defaultCharacterProperties;
        /// <summary>
        /// Default Character AI infos.
        /// </summary>
        public CharacterAIProperties DefaultCharacterAIProperties => defaultCharacterAIproperties;
        /// <summary>
        /// List of preconfigured Characters AI.
        /// </summary>
        public List<CharacterAIScriptableObject> CharacterScriptableObjects => characterScriptableObjects;

        public int PlayersNumberMin => playersNumberMin;
        public int PlayersNumberMax => playersNumberMax;
        public int DefaultPlayersNumber => defaultPlayersNumber;

        public DataManager.CardSendingType DefaultCardSendingMethod => defaultCardSendingMethod;
        #endregion
    }
}