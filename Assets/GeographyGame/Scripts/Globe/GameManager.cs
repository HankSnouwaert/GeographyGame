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
        private IGlobeManager globeManager; 
        private IErrorHandler ErrorHandler { get; set; }
        public GameObject touristManagerObject;
        public ITouristManager TouristManager { get; set; }
        public GameObject cameraManagerObject;
        public ICameraManager CameraManager { get; set; }
        //public GameObject playerPrefab;
        public GameObject errorPanel;
        public InventoryUI inventoryUI;
        public AudioSource dropOffSuccess;
        public AudioSource dropOffFailure;
        //TO BE SORTED
        private ICellClicker cellClicker;
        public ICellCursorInterface CellCursorInterface { get; set; }
        public List<ITurnBasedObject> TurnBasedObjects { get; set; } = new List<ITurnBasedObject>();
        //Panel Messages
        private Text errorMessage;  //Error Manager
        private InputField stackTraceInputField; //Error Manager
        //Prefabs
        //private InventoryTourist touristPrefab; //Tourist Manager
        //Counters
        private int globalTurnCounter = 0; 
        //private int touristCounter = 0; //Tourist Manager
        public int score = 0;
        //private int touristsInCurrentRegion = -2;  //This number is the starting number of tourists * -1  (Tourist Manager)
        private int turnsRemaining = 250;
        //private int touristImageIndex = 0; //Tourist Manager
        //Flags
        public bool GamePaused { get; set; } = false;
        public bool GameMenuOpen { get; set; } = false;
        public ErrorState errorState = ErrorState.close_window;  //Error Manager
        //Game Settings
        //private int touristSpawnRate = 10; //Number of rounds for a tourist to spawn  (Tourist Manager)
        //public int TrackingTime { get; } = 10; //Number of rounds a tourist is remembered  (Tourist Manager)
        //public const int MIN_TIME_IN_REGION = 5;  //Tourist Manager
        //public const int MAX_TIME_IN_REGION = 10;  //Tourist Manager
        //In-Game Objects
        public PlayerCharacter player;
        private SelectableObject selectedObject;
        public SelectableObject SelectedObject
        {
            get { return selectedObject; }
            set
            {
                selectedObject = value;
                if (selectedObject != null)
                    cellClicker.NewObjectSelected = true;
            }
        }
        public SelectableObject HighlightedObject { get; set; } = null;

        //public Dictionary<string, MappableObject> mappedObjects = new Dictionary<string, MappableObject>();  //Globe Manager: GlobeInfo
        
        //Tourist Tracking Lists
        //public List<int> RecentProvinceDestinations { get; set; } = new List<int>();  //Tourist Manager
        //public List<string> RecentLandmarkDestinations { get; set; } = new List<string>();  //Tourist Manager
        //public List<int> RecentCountryDestinations { get; set; } = new List<int>();  //Tourist Manager
        //Map Regions
        //public List<TouristRegion> touristRegions = new List<TouristRegion>();  //Tourist Manager
        //public TouristRegion CurrentRegion { get; set; }  //Tourist Manager
        //private List<TouristRegion> regionsVisited = new List<TouristRegion>();  //Tourist Manager
        //Landmark Lists
        //public Dictionary<string, Landmark> culturalLandmarks = new Dictionary<string, Landmark>();  //Globe Manager: GlobeInfo
        //public Dictionary<string, Landmark> CulturalLandmarksByName { get; } = new Dictionary<string, Landmark>(); //Globe Manager: GlobeInfo
        //MACROS 
        //Province Attributes
        public const int NUMBER_OF_PROVINCE_ATTRIBUTES = 3; 
        public const int POLITICAL_PROVINCE = 0;
        public const int TERRAIN = 1;
        public const int CLIMATE = 2;
        //Mount Points
        public const int START_POINT = 0;
        public const int NATURAL_POINT = 1;
        public const int CULTURAL_POINT = 2;
        //public const string CELL_PLAYER = "Player";
        //Error States
        public const int CLOSE_WINDOW = 0;
        public const int RESTART_SCENE = 1;
        public const int CLOSE_APPLICATION = 2;

        //Tourist Image Management
        //private string[] touristImageFiles; //Tourist Manager
        //private const int NUMBER_OF_TOURIST_IMAGES = 8; //Tourist Manager

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
            globeManager = interfaceFactory.GlobeManager;
            ErrorHandler = interfaceFactory.ErrorHandler;
            UIManager = interfaceFactory.UIManager;
            cellClicker = globeManager.CellCursorInterface.CellClicker;
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
                        selectedObject.Deselected();
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
        /*
        /// <summary> 
        /// Display a popup notification with a given message
        /// </summary>
        /// <param name="displayText"></param> The text to be displayed on the pop up
        /// are updated by
        public void DisplayPopUp(string displayText)
        {
            popUpPanel.SetActive(true);
            popUpMessage.text = displayText;
        }

        /// <summary> 
        /// Close active pop up
        /// </summary>
        public void ClosePopUp()
        {
            popUpPanel.SetActive(false);
        }
        */
        /// <summary> 
        /// Display an error popup with a given error message
        /// </summary>
        /// <param name="errorText"></param> The error text to be displayed on the pop up
        /// are updated by
        public void DisplayError(string errorText, string stackTraceText)
        {
            errorPanel.SetActive(true);
            errorMessage.text = "Error: " + errorText;
            stackTraceInputField.text = stackTraceText;
        }

        /*
        public void ErrorButton()
        {
            errorPanel.SetActive(false);
            ClosingGUIPanel = true;
            switch (errorState)
            {
                case (ErrorState.close_window):
                    break;
                case (ErrorState.restart_scene):
                    GameReset();
                    break;
                case (ErrorState.close_application):
                    ExitGame();
                    break;
                default:
                    break;
            }   
        }
        */

        public void DropOff(bool success)
        {
            /*  This will be for drop off sound effects
            if (success)
                dropOffSuccess.Play();
            else
                dropOffFailure.Play();
            */    
        }
        
    }
}
