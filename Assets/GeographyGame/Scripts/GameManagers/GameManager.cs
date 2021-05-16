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
        private GameObject playerManagerObject;
        [SerializeField]
        private GameObject touristManagerObject;
        [SerializeField]
        private GameObject cameraManagerObject;
        [SerializeField]
        private GameObject turnsManagerObject;
        [SerializeField]
        private GameObject scoreManagerObject;
        //Child Interfaces
        public IPlayerManager PlayerManager { get; protected set; }
        public ITouristManager TouristManager { get; protected set; }
        public ICameraManager CameraManager { get; protected set; }
        public ITurnsManager TurnsManager { get; protected set; }
        public IScoreManager ScoreManager { get; protected set; }
        //Local Interface References
        private IUIManager uiManager;
        private IGlobeManager globeManager;
        private ICellClicker cellClicker;
        private IGameMenuUI gameMenuUI;
        //Flags
        public bool GamePaused { get; set; } = false;
        //In-Game Objects
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
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;
        private bool componentMissing = false;

        #endregion

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if(interfaceFactory == null)
                gameObject.SetActive(false);
            try
            {
                PlayerManager = playerManagerObject.GetComponent(typeof(IPlayerManager)) as IPlayerManager;
                if (PlayerManager == null)
                    componentMissing = true;
                TouristManager = touristManagerObject.GetComponent(typeof(ITouristManager)) as ITouristManager;
                if(TouristManager == null)
                    componentMissing = true;
                CameraManager = cameraManagerObject.GetComponent(typeof(ICameraManager)) as ICameraManager;
                if (CameraManager == null)
                    componentMissing = true;
                TurnsManager = turnsManagerObject.GetComponent(typeof(ITurnsManager)) as ITurnsManager;
                if (TurnsManager == null)
                    componentMissing = true;
                ScoreManager = scoreManagerObject.GetComponent(typeof(IScoreManager)) as IScoreManager;
                if (ScoreManager == null)
                    componentMissing = true;
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
                    gameMenuUI.CloseUI();
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
