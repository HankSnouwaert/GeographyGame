using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SpeedTutorMainMenuSystem;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace WPM
{
    public class GameManager : MonoBehaviour
    {
        #region Variable Declaration 
        //[Header("Player Components")]
        private IUIManager UIManager { get; set; }
        private IGlobeManager GlobeManager { get; set; } 
        private IErrorHandler ErrorHandler { get; set; }
        public GameObject touristManagerObject;
        public ITouristManager TouristManager { get; set; }
        public GameObject cameraManagerObject;
        public ICameraManager CameraManager { get; set; }
        //TO BE SORTED
        private ICellClicker cellClicker;
        public ICellCursorInterface CellCursorInterface { get; set; }
        public List<ITurnBasedObject> TurnBasedObjects { get; set; } = new List<ITurnBasedObject>();
        //Counters
        private int globalTurnCounter = 0; 
        public int score = 0;
        private int turnsRemaining = 250;
        //Flags
        public bool GamePaused { get; set; } = false;
        public bool GameMenuOpen { get; set; } = false;
        //Game Settings
        //In-Game Objects
        public IPlayerCharacter Player { get; set; }
        private ISelectableObject selectedObject;
        public ISelectableObject SelectedObject
        {
            get { return selectedObject; }
            set
            {
                selectedObject = value;
                if (selectedObject != null)
                    cellClicker.NewObjectSelected = true;
            }
        }
        public ISelectableObject HighlightedObject { get; set; } = null;


        static GameManager _instance;

        #endregion

        /// <summary>
        /// Instance of the game manager. Use this property to access World Map functionality.
        /// </summary>
        public static GameManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                    if (_instance == null)
                    {
                        Debug.LogWarning("'GameManger' GameObject could not be found in the scene. Make sure it's created with this name before using any map functionality.");
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            TouristManager = touristManagerObject.GetComponent(typeof(ITouristManager)) as ITouristManager;
            CameraManager = cameraManagerObject.GetComponent(typeof(ICameraManager)) as ICameraManager;
        }

        void Start()
        {
            InterfaceFactory interfaceFactory = FindObjectOfType<InterfaceFactory>();
            GlobeManager = interfaceFactory.GlobeManager;
            ErrorHandler = interfaceFactory.ErrorHandler;
            UIManager = interfaceFactory.UIManager;
            cellClicker = GlobeManager.CellCursorInterface.CellClicker;
        }

        void Update()
        {
            //Esc out of Selected Objects and UI Menus
            if (Input.GetKeyDown("escape"))
            {
                if (GameMenuOpen)
                    UIManager.ExitCurrentUI();
                else
                {
                    if (selectedObject != null)
                        selectedObject.Deselect();
                    else
                    {
                        UIManager.GameMenuUI.OpenUI();
                        GameMenuOpen = true;
                        GamePaused = true;
                    }
                }      
            }
        }

        /// <summary>
        /// Called whenever a new turn happens in game. Multiple turns can pass at once.
        /// Inputs:
        ///     turns: How many turns are passing
        /// </summary>
        public void NextTurn(int turns)
        {
            globalTurnCounter = globalTurnCounter + turns;
            UpdateRemainingTurns(turns*-1);
            //Run any end of turn scripts for the rest of the game's objects
            foreach(ITurnBasedObject turnBasedObject in TurnBasedObjects)
            {
                turnBasedObject.EndOfTurn(turns);
            }
        }

        /// <summary> 
        /// Update the turns remaining until the game ends and check if game has ended
        /// </summary>
        /// <param name="turnModification"></param> The number of turns the reminaing turns
        /// are updated by
        private void UpdateRemainingTurns(int turnModification)
        {
            turnsRemaining = turnsRemaining + turnModification;
            if (turnsRemaining <= 0)
            {
                turnsRemaining = 0;
                GameOver();
            }
            UIManager.TurnsUI.UpdateDisplayedRemainingTurns(turnsRemaining);
        }

        /// <summary>
        ///  Update the games current score
        /// </summary>
        /// <param name="scoreModification"></param> The amount the score should be changed by>
        /// <returns></returns> 
        public void UpdateScore(int scoreModification)
        {
            score = score + scoreModification;
            UIManager.ScoreUI.UpdateDisplayedScore(score);
        }

        /// <summary> 
        /// Called when game ends
        /// </summary>
        public void GameOver()
        {
            UIManager.GameOver();
            GamePaused = true;
        }

        /// <summary> 
        /// Resets the game by reloading the scene
        /// </summary>
        public void GameReset()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary> 
        /// Exit the application
        /// </summary>
        public void ExitGame()
        {
            Application.Quit();
        }

        public void ResumeGame()
        {
            GamePaused = false;
            GameMenuOpen = false;
        }
    }
}
