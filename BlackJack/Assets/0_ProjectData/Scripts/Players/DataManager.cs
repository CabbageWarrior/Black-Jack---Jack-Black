using CabbageSoft.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

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
        public CharacterScriptableObject CharacterScriptableObject;

        /// <summary>
        /// Player's name.
        /// </summary>
        public string Name;
        /// <summary>
        /// Player's percentage of risk.
        /// </summary>
        public float RiskPercentage;
        /// <summary>
        /// Player's minimum score from which start to calculate the risk.
        /// </summary>
        public int RiskCalcMinValue;
    }
    #endregion

    #region Statics
    /// <summary>
    /// Used to know if the DataManager has been created.
    /// </summary>
    private static bool created = false;
    #endregion

    #region Inspector Infos
    [Header("Default Configuration")]
    /// <summary>
    /// Default Player Index.
    /// </summary>
    [SerializeField] private int defaultNumber = 0;
    /// <summary>
    /// Default Player Name.
    /// </summary>
    [SerializeField] private string defaultName = "";
    /// <summary>
    /// Default Player Risk Percentage.
    /// </summary>
    [SerializeField] private float defaultRiskPercentage = 50f;
    /// <summary>
    /// Default Player Minimum Score after which calculate risk.
    /// </summary>
    [SerializeField] private int defaultRiskMinValue = 10;
    /// <summary>
    /// Default Player Sprite.
    /// </summary>
    [SerializeField] private Sprite defaultSprite = default;

    [Header("Preconfigured data")]
    [SerializeField] private List<CharacterScriptableObject> characterScriptableObjects = new List<CharacterScriptableObject>();
    #endregion

    #region Private Stuff
    /// <summary>
    /// List used to register informations about the players that the system must initialize in game.
    /// </summary>
    private List<PlayerAIInitData> playersToInit = new List<PlayerAIInitData>();
    #endregion

    #region Properties
    /// <summary>
    /// Default Player Index.
    /// </summary>
    public int DefaultNumber => defaultNumber;
    /// <summary>
    /// Default Player Name.
    /// </summary>
    public string DefaultName => defaultName;
    /// <summary>
    /// Default Player Risk Percentage.
    /// </summary>
    public float DefaultRiskPercentage => defaultRiskPercentage;
    /// <summary>
    /// Default Player Minimum Score after which calculate risk.
    /// </summary>
    public int DefaultRiskMinValue => defaultRiskMinValue;
    /// <summary>
    /// Default Player Sprite.
    /// </summary>
    public Sprite DefaultPortraitSprite => defaultSprite;

    public List<CharacterScriptableObject> CharacterScriptableObjects => characterScriptableObjects;

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
