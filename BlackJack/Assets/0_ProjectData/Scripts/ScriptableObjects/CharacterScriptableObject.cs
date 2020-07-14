using UnityEngine;

namespace CabbageSoft.ScriptableObjects
{
    /// <summary>
    /// <para>Class that contains the configuration of a character.</para>
    /// <para>See the <see cref="CharacterScriptableObjectEditor"/> for more details about the Inspector.</para>
    /// </summary>
    [CreateAssetMenu(fileName = "Character", menuName = "CabbageSoft/ScriptableObjects/Character", order = 1)]
    public class CharacterScriptableObject : ScriptableObject
    {
        #region Inspector Infos
        // Identity
        [SerializeField] private string characterName = "Character";
        [SerializeField] private Sprite portraitSprite = default;
        [SerializeField] private Sprite frontFaceSprite = default;

        // Statistics
        /// <summary>
        /// Player's percentage of risk.
        /// </summary>
        [SerializeField, Range(0f, 100f)] private float riskPercentage = 50f;
        /// <summary>
        /// Player's minimum score from which start to calculate the risk.
        /// </summary>
        [SerializeField, Range(0, 20)] private int riskCalcMinValue = 10;
        #endregion

        #region Properties
        public string CharacterName => characterName;
        public Sprite PortraitSprite => portraitSprite;
        public Sprite FrontFaceSprite => frontFaceSprite;

        /// <summary>
        /// Player's percentage of risk.
        /// </summary>
        public float RiskPercentage => riskPercentage;
        /// <summary>
        /// Player's minimum score from which start to calculate the risk.
        /// </summary>
        public int RiskCalcMinValue => riskCalcMinValue;
        #endregion


    }
}