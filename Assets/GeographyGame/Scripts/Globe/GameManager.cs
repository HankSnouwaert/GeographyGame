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
    public class GameManager : MonoBehaviour, IGameManager
    {
        #region Variable Declaration
        [Header("Child Objects")]
        [SerializeField]
        private GameObject touristManagerObject;
        [SerializeField]
        private GameObject cameraManagerObject;
        [SerializeField]
        private GameObject turnsManagerObject;
        [SerializeField]
        private GameObject scoreManagerObject;
        //Child Interfaces
        public ITouristManager TouristManager { get; protected set; }
        public ICameraManager CameraManager { get; protected set; }
        public ITurnsManager TurnsManager { get; protected set; }
        public IScoreManager ScoreManager { get; protected set; }
        //Private Interfaces
        private IUIManager uiManager;
        private IGlobeManager globeManager;
        private IErrorHandler errorHandler;
        private ICellClicker cellClicker;
        private IGameMenuUI gameMenuUI;
        //Flags
        public bool GamePaused { get; set; } = false;
        private bool componentMissing = false;
        //In-Game Objects
        public IPlayerCharacter Player { get; set; }
        private InterfaceFactory interfaceFactory;
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

        #endregion

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if(interfaceFactory == null)
                gameObject.SetActive(false);
            try
            {
                TouristManager = touristManagerObject.GetComponent(typeof(ITouristManager)) as ITouristManager;
                CameraManager = cameraManagerObject.GetComponent(typeof(ICameraManager)) as ICameraManager;
                TurnsManager = turnsManagerObject.GetComponent(typeof(ITurnsManager)) as ITurnsManager;
                ScoreManager = scoreManagerObject.GetComponent(typeof(IScoreManager)) as IScoreManager;
            }
            catch
            {
                componentMissing = true;
            }
            
            SelectedObject = null;
        }

        void Start()
        {
            globeManager = interfaceFactory.GlobeManager;
            errorHandler = interfaceFactory.ErrorHandler;
            uiManager = interfaceFactory.UIManager;
            if(globeManager == null || errorHandler == null || uiManager == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                if (componentMissing)
                {
                    errorHandler.ReportError("GameManager component missing", ErrorState.restart_scene);
                }

                ICellCursorInterface cellCursorInterface = globeManager.CellCursorInterface;
                if (cellCursorInterface == null)
                    errorHandler.ReportError("CellCursorInterface Missing", ErrorState.restart_scene);

                cellClicker = cellCursorInterface.CellClicker;
                if (cellClicker == null)
                    errorHandler.ReportError("CellClicker Missing", ErrorState.restart_scene);

                gameMenuUI = uiManager.GameMenuUI;
                if (gameMenuUI == null)
                    errorHandler.ReportError("Game Menu UI Missing", ErrorState.restart_scene);
            }
        }

        void Update()
        {
            //Esc out of Selected Objects and UI Menus
            if (Input.GetKeyDown("escape"))
            {
                if (gameMenuUI.UIOpen)
                    uiManager.ExitCurrentUI();
                else
                {
                    if (selectedObject != null)
                        selectedObject.Deselect();
                    else
                    {
                        gameMenuUI.OpenUI();
                        GamePaused = true;
                    }
                }      
            }
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

        /// <summary> 
        /// Called when the game is unpaused
        /// </summary>
        public void ResumeGame()
        {
            GamePaused = false;
        }
    }
}
