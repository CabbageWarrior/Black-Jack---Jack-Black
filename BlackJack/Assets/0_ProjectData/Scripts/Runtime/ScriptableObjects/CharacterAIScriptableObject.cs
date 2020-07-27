using UnityEngine;
using CabbageSoft.BlackJack.Properties;

namespace CabbageSoft.BlackJack.ScriptableObjects
{
    /// <summary>
    /// <para>Class that contains the configuration of a character.</para>
    /// <para>See the <see cref="CharacterScriptableObjectEditor"/> for more details about the Inspector.</para>
    /// </summary>
    [CreateAssetMenu(fileName = "Character", menuName = "CabbageSoft/ScriptableObjects/CharacterAI", order = 11)]
    public class CharacterAIScriptableObject : ScriptableObject
    {
        #region Inspector Infos
        /// <summary>
        /// Default Character AI properties.
        /// </summary>
        [Header("==> Characters AI", order = 1)]
        [SerializeField] private CharacterAIProperties properties = default;
        #endregion

        #region Properties
        /// <summary>
        /// Default Character AI properties.
        /// </summary>
        public CharacterAIProperties Properties => properties;
        #endregion
    }
}