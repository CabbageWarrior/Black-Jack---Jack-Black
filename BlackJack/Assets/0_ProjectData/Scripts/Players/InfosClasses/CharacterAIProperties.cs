using UnityEngine;
using CabbageSoft.BlackJack.Characters;

namespace CabbageSoft.BlackJack.Properties
{
    /// <summary>
    /// CLass that defines the properties about a Character AI.
    /// </summary>
    [System.Serializable]
    public class CharacterAIProperties : CharacterProperties
    {
        #region Inspector Infos
        /// <summary>
        /// Character's Risk Percentage.
        /// </summary>
        [Header("Character AI Properties")]
        [SerializeField, Range(0, 100)] protected float riskPercentage = 50f;
        /// <summary>
        /// Character's Minimum Score after which calculate risk.
        /// </summary>
        [SerializeField, Range(0, 20)] protected int riskCalcMinScore = 10;
        /// <summary>
        /// Character's Portrait Sprite.
        /// </summary>
        [SerializeField, SpriteWithPreview] protected Sprite portraitSprite = default;
        /// <summary>
        /// Character's Face Sprite.
        /// </summary>
        [SerializeField, SpriteWithPreview] protected Sprite frontFaceSprite = default;
        /// <summary>
        /// Character's Model Prefab.
        /// </summary>
        [SerializeField] protected CharacterModelController characterModelPrefab = default;
        #endregion

        #region Properties
        /// <summary>
        /// Character's Risk Percentage.
        /// </summary>
        public float RiskPercentage
        {
            get => riskPercentage;
            set => riskPercentage = value;
        }
        /// <summary>
        /// Character's Minimum Score after which calculate risk.
        /// </summary>
        public int RiskCalcMinScore
        {
            get => riskCalcMinScore;
            set => riskCalcMinScore = value;
        }
        /// <summary>
        /// Character's Portrait Sprite.
        /// </summary>
        public Sprite PortraitSprite
        {
            get => portraitSprite;
            set => portraitSprite = value;
        }
        /// <summary>
        /// Character's Face Sprite.
        /// </summary>
        public Sprite FrontFaceSprite
        {
            get => frontFaceSprite;
            set => frontFaceSprite = value;
        }
        /// <summary>
        /// Character's Model Prefab.
        /// </summary>
        public CharacterModelController CharacterModelPrefab
        {
            get => characterModelPrefab;
            set => characterModelPrefab = value;
        }
        #endregion

        public new CharacterAIProperties Clone()
        {
            CharacterAIProperties newCharacterAIProperties = new CharacterAIProperties();
            newCharacterAIProperties.characterName = this.CharacterName;
            newCharacterAIProperties.riskPercentage = this.RiskPercentage;
            newCharacterAIProperties.riskCalcMinScore = this.RiskCalcMinScore;
            newCharacterAIProperties.portraitSprite = this.PortraitSprite;
            newCharacterAIProperties.frontFaceSprite = this.FrontFaceSprite;
            newCharacterAIProperties.characterModelPrefab = this.CharacterModelPrefab;

            return newCharacterAIProperties;
        }
    }
}