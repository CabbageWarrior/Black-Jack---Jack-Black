using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using CabbageSoft.BlackJack.DeckManagement;

namespace CabbageSoft.BlackJack
{
    [RequireComponent(typeof(PlayersManager))]
    public class GameManager : MonoBehaviour
    {
        #region Enums
        /// <summary>
        /// State of the game.
        /// </summary>
        public enum TurnState
        {
            Game = 0,
            End = 1
        }
        #endregion

        #region Consts
        /// <summary>
        /// The amount of maximum points.
        /// </summary>
        public const int BlackJackPoints = 21;
        #endregion

        #region Statics
        /// <summary>
        /// Singleton instance of GameManager.
        /// </summary>
        public static GameManager instance = null;
        /// <summary>
        /// Current state of the match.
        /// </summary>
        public static TurnState currentState = TurnState.End;
        #endregion

        #region Public
        /// <summary>
        /// Deck in scene.
        /// </summary>
        public Deck deck = default;
        #endregion

        #region Private
        /// <summary>
        /// PlayersManager Component reference.
        /// </summary>
        private PlayersManager playersManager = default;
        #endregion

        #region Properties
        /// <summary>
        /// DataManager getter.
        /// </summary>
        public DataManager DataManager { get; private set; }
        /// <summary>
        /// Jack Black Deck.
        /// </summary>
        public static Deck StaticDeckRef { get; private set; }
        #endregion

        #region Events from Inspector
        /// <summary>
        /// What happens when DataManager finishes to load.
        /// </summary>
        public UnityEvent OnDataManagerLoadComplete;
        /// <summary>
        /// What happens when current state becomes Game.
        /// </summary>
        [Space]
        public UnityEvent OnSetStateGame;
        /// <summary>
        /// What happens when current state becomes End.
        /// </summary>
        public UnityEvent OnSetStateEnd;
        #endregion

        #region Methods
        #region MonoBehaviour Methods
        /// <summary>
        /// Component Awake method.
        /// </summary>
        private void Awake()
        {
            // If the instance is already set and it is an object different
            // than this, then this object is destroyed.
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            //GameObject instance Singleton initialization.
            instance = this;

            // Initializing the reference to the PlayersManager component.
            playersManager = GetComponent<PlayersManager>();

            // Initializing the static reference of the current Deck.
            StaticDeckRef = deck;
        }

        /// <summary>
        /// Component Start method.
        /// </summary>
        private IEnumerator Start()
        {
            // Waiting to find the DataManager reference. DataReference is a "DontDestroyOnLoad" object from the Menu scene.
            while (DataManager == null)
            {
                DataManager = GameObject.FindObjectOfType<DataManager>();
                yield return null;
            }

            OnDataManagerLoadComplete?.Invoke();
            yield return null;
        }
        #endregion

        #region Public
        /// <summary>
        /// Turn initialization method.
        /// </summary>
        public void InitializeTurn()
        {
            StartCoroutine(InitializeTurn_Coroutine());
        }
        private IEnumerator InitializeTurn_Coroutine()
        {
            // Need to reset players' informations.
            yield return StartCoroutine(playersManager.ResetPlayers());

            // Resetting cards' positions in the deck.
            deck.SetCardsPositionByOrder();
        }

        /// <summary>
        /// Sets the turn's current state to Game.
        /// </summary>
        public void SetStateGame()
        {
            currentState = TurnState.Game;
            OnSetStateGame?.Invoke();
        }
        /// <summary>
        /// Sets the turn's current state to End.
        /// </summary>
        public void SetStateEnd()
        {
            currentState = TurnState.End;
            OnSetStateEnd?.Invoke();
        }

        /// <summary>
        /// Interrupts the game and loads the Menu scene.
        /// </summary>
        public void ReturnToMainMenu()
        {
            SetStateEnd();
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
        #endregion
        #endregion
    }
}
