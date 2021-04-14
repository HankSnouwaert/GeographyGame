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
        [Header("Child Objects")]
        [SerializeField]
        private GameObject touristManagerObject;
        [SerializeField]
        private GameObject cameraManagerObject;
        //Child Interfaces
        public ITouristManager TouristManager { get; protected set; }
        public ICameraManager CameraManager { get; protected set; }
        //Private Interfaces
        private IUIManager uiManager;
        private IGlobeManager globeManager;
        private IErrorHandler errorHandler;
        private ICellClicker cellClicker;
        //Counters
        private int globalTurnCounter = 0; 
        public int Score { get; protected set; } = 0;
        public int TurnsRemaining { get; protected set; } = 250;
        //Flags
        public bool GamePaused { get; set; } = false;
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
        public List<ITurnBasedObject> TurnBasedObjects { get; set; } = new List<ITurnBasedObject>();

        #endregion

        private void Awake()
        {
            TouristManager = touristManagerObject.GetComponent(typeof(ITouristManager)) as ITouristManager;
            CameraManager = cameraManagerObject.GetComponent(typeof(ICameraManager)) as ICameraManager;
            SelectedObject = null;
        }

        void Start()
        {
            InterfaceFactory interfaceFactory = FindObjectOfType<InterfaceFactory>();
            globeManager = interfaceFactory.GlobeManager;
            errorHandler = interfaceFactory.ErrorHandler;
            uiManager = interfaceFactory.UIManager;
            cellClicker = globeManager.CellCursorInterface.CellClicker;
        }

        void Update()
        {
            //Esc out of Selected Objects and UI Menus
            if (Input.GetKeyDown("escape"))
            {
                if (uiManager.GameMenuUI.UIOpen)
                    uiManager.ExitCurrentUI();
                else
                {
                    if (selectedObject != null)
                        selectedObject.Deselect();
                    else
                    {
                        uiManager.GameMenuUI.OpenUI();
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
            TurnsRemaining = TurnsRemaining + turnModification;
            if (TurnsRemaining <= 0)
            {
                TurnsRemaining = 0;
                GameOver();
            }
            uiManager.TurnsUI.UpdateDisplayedRemainingTurns(TurnsRemaining);
        }

        /// <summary>
        ///  Update the games current score
        /// </summary>
        /// <param name="scoreModification"></param> The amount the score should be changed by>
        /// <returns></returns> 
        public void UpdateScore(int scoreModification)
        {
            Score = Score + scoreModification;
            uiManager.ScoreUI.UpdateDisplayedScore(Score);
        }

        /// <summary> 
        /// Called when game ends
        /// </summary>
        public void GameOver()
        {
            uiManager.GameOver();
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
        }
    }
}
