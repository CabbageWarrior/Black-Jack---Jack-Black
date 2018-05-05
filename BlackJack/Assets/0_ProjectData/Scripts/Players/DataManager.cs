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

    #region Properties
    #region Private
    /// <summary>
    /// List used to register informations about the players that the system must initialize in game.
    /// </summary>
    private List<PlayerAIInitData> playersToInit;

    /// <summary>
    /// The dealer's name.
    /// </summary>
    private string dealerName;
    /// <summary>
    /// The dealer's name.
    /// </summary>
    public string DealerName
    {
        get
        {
            return dealerName;
        }

        set
        {
            dealerName = value;
        }
    }

    /// <summary>
    /// The mode in which cards are sent to players.
    /// </summary>
    private CardSendingType cardSendingMode;
    /// <summary>
    /// The mode in which cards are sent to players.
    /// </summary>
    public CardSendingType CardSendingMode
    {
        get
        {
            return cardSendingMode;
        }

        set
        {
            cardSendingMode = value;
        }
    }
    #endregion
    #endregion

    #region Methods
    #region MonoBehaviour Methods
    /// <summary>
    /// Component Awake method.
    /// </summary>
    void Awake()
    {
        // If the object is not undestroyable, the system sets it as undestroyable.
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
    }

    /// <summary>
    /// Component Start method.
    /// </summary>
    void Start()
    {
        // Initializing the playersToInit List.
        if (playersToInit == null) playersToInit = new List<PlayerAIInitData>();
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
