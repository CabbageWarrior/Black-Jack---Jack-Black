using System.Collections.Generic;
using UnityEngine;
using CabbageSoft.JackBlack.Properties;

namespace CabbageSoft.JackBlack.ScriptableObjects
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
        [Header("==> Characters AI", order = 1)]
        [SerializeField] private CharacterAIProperties defaultCharacterAIproperties = default;
        /// <summary>
        /// List of preconfigured Characters AI.
        /// </summary>
        [Space]
        [SerializeField] private List<CharacterAIScriptableObject> characterScriptableObjects = new List<CharacterAIScriptableObject>();
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
        #endregion
    }
}