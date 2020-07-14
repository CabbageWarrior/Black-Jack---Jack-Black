using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class PlayersManager : MonoBehaviour
{
    #region Statics
    /// <summary>
    /// AI Players count.
    /// </summary>
    public static int AIPlayersCount = 0;
    /// <summary>
    /// Reference to the current player.
    /// </summary>
    public static Player currentPlayer;
    /// <summary>
    /// Reference to the current player's index.
    /// </summary>
    private static int currentPlayerIndex = 0;
    #endregion

    #region Properties
    #region Public
    /// <summary>
    /// Maximum angle of distance between players.
    /// </summary>
    public float playersDistanceAngle = 45f;
    /// <summary>
    /// Maximum angle for placement of players.
    /// </summary>
    public float maxArrangementAngle = 180f;
    /// <summary>
    /// Prefab of the AI Player.
    /// </summary>
    [Space]
    public GameObject playerAIPrefab;
    /// <summary>
    /// Container of AI Players.
    /// </summary>
    public GameObject PlayersAIGroup;
    /// <summary>
    /// Reference to the Dealer.
    /// </summary>
    public PlayerDealer playerDealer;
    #endregion

    #region Private
    /// <summary>
    /// List of AI Players.
    /// </summary>
    private List<PlayerAI> AIPlayers;
    /// <summary>
    /// Reference to the GameManager.
    /// </summary>
    private GameManager gameManager;
    #endregion
    #endregion

    #region Methods
    #region Public
    /// <summary>
    /// Gets the AI Players List.
    /// </summary>
    /// <returns>List of AI Players.</returns>
    public List<PlayerAI> GetAIPlayers()
    {
        return AIPlayers;
    }
    /// <summary>
    /// Initializes the PlayersManager Component.
    /// </summary>
    public void InitializePlayersManager()
    {
        // Gets the GameManager reference.
        gameManager = GetComponent<GameManager>();

        // Initializes the AIPlayers List.
        AIPlayers = new List<PlayerAI>();

        // Clears the content of the PlayersAIGroup GameObject.
        foreach (Transform child in PlayersAIGroup.transform)
        {
            Destroy(child.gameObject);
        }

        // For each data in DataManager, the system creates an AI Player with the correct settings. The new player is added to PlayersAIGroup.
        List<DataManager.PlayerAIInitData> playersToInit = gameManager.DataManager.GetPlayersToInit();
        DataManager.PlayerAIInitData data;
        GameObject currentPlayer;
        PlayerAI currentPlayerAI;
        for (int i = 0; i < playersToInit.Count; i++)
        {
            data = playersToInit[i];
            currentPlayer = Instantiate(playerAIPrefab, PlayersAIGroup.transform);
            currentPlayerAI = currentPlayer.GetComponent<PlayerAI>();

            currentPlayerAI.playerName = data.Name;
            currentPlayerAI.percentageOfRisk = data.RiskPercentage;
            currentPlayerAI.minValueForRiskCalculation = data.RiskCalcMinValue;

            // Visual update.
            currentPlayerAI.nameText.text = currentPlayerAI.playerName;

            AIPlayers.Add(currentPlayerAI);
        }

        // The name of the dealer is updated with the name set in DataManager.
        playerDealer.playerName = gameManager.DataManager.DealerName;
        // Visual update.
        playerDealer.nameText.text = playerDealer.playerName;

        AIPlayersCount = AIPlayers.Count;

        // Setting up the real angle between players.
        float realPlayersDistanceAngle = playersDistanceAngle;
        if (AIPlayersCount == 1)
        {
            realPlayersDistanceAngle = maxArrangementAngle / 2;
        }
        else if (realPlayersDistanceAngle * AIPlayersCount > maxArrangementAngle)
        {
            realPlayersDistanceAngle = maxArrangementAngle / (AIPlayersCount - 1);
        }

        // Every player is located in its proper position.
        // For each player, the system initializes its OnFinish delegate value.
        PlayerAI playerAI;
        for (int i = 0; i < AIPlayersCount; i++)
        {
            playerAI = AIPlayers[i];
            playerAI.OnFinish += MoveToNextPlayer;

            playerAI.transform.rotation = Quaternion.Euler(
                0f,
                (realPlayersDistanceAngle * i) - (realPlayersDistanceAngle * (AIPlayersCount - 1) / 2),
                0f
            );

            playerAI.SetInfoCanvasRotation();
        }
        // Setting the delegate of the Dealer.
        playerDealer.OnFinish += CheckWinners;

        // Every single AI Player's score and visual infos are resetted.
        StartCoroutine(ResetPlayers());
    }
    /// <summary>
    /// Method that resets every single AI Player.
    /// </summary>
    /// <returns>IEnumerator value.</returns>
    public IEnumerator ResetPlayers()
    {
        // Every single AI Player and the Dealer are resetted.
        for (int i = 0; i < AIPlayers.Count; i++)
        {
            yield return StartCoroutine(AIPlayers[i].ResetInfos());
        }
        yield return StartCoroutine(playerDealer.ResetInfos());

        switch (GameManager.instance.DataManager.CardSendingMode)
        {
            case DataManager.CardSendingType.SingleClick:
                // Reset first player
                currentPlayerIndex = 0;
                currentPlayer = AIPlayers[currentPlayerIndex];
                currentPlayer.StartTurn();
                break;
            case DataManager.CardSendingType.DragAndDrop:
                currentPlayer = null;
                foreach (PlayerAI playerAI in AIPlayers)
                {
                    playerAI.StartTurn();
                }
                break;
            default:
                // Nothing to do.
                break;
        }

        // Activate the Game phase.
        gameManager.SetStateGame();

        yield return null;
    }
    /// <summary>
    /// For each AI Player, the system checks the winners.
    /// </summary>
    public void CheckWinners()
    {
        // Getting infos about the Dealer: score and if he has a BlackJack.
        int dealerScore = playerDealer.CurrentScore;
        bool isDealerBlackJack = playerDealer.IsBlackJack;

        for (int i = 0; i < AIPlayers.Count; i++)
        {
            AIPlayers[i].CheckScore(dealerScore, isDealerBlackJack);
        }
    }
    #endregion

    #region Private
    /// <summary>
    /// Sets the next player as the current player.
    /// </summary>
    private void MoveToNextPlayer()
    {
        switch (GameManager.instance.DataManager.CardSendingMode)
        {
            case DataManager.CardSendingType.SingleClick:
                currentPlayerIndex++;
                if (currentPlayerIndex < AIPlayers.Count)
                {
                    currentPlayer = AIPlayers[currentPlayerIndex];
                }
                else
                {
                    currentPlayer = playerDealer;
                }

                currentPlayer.StartTurn();
                break;
            case DataManager.CardSendingType.DragAndDrop:
                if (AIPlayers.Find(x => x.CurrentState == Player.State.WaitingForCard) == null)
                {
                    playerDealer.StartTurn();
                }
                break;
            default:
                // Nothing to do.
                break;
        }
    }
    #endregion
    #endregion
}
