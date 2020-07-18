using UnityEngine;

namespace CabbageSoft.JackBlack.Properties
{
    /// <summary>
    /// CLass that defines the properties about a Character.
    /// </summary>
    [System.Serializable]
    public class CharacterProperties
    {
        #region Inspector Infos
        /// <summary>
        /// The Name of the Character.
        /// </summary>
        [Header("Character Properties")]
        [SerializeField] protected string characterName = string.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// The Name of the Character.
        /// </summary>
        public string CharacterName
        {
            get => characterName;
            set => characterName = value;
        }
        #endregion

        public CharacterProperties Clone()
        {
            return new CharacterProperties()
            {
                characterName = CharacterName
            };
        }
    }
}