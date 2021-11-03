using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class TutorialManager : MonoBehaviour, ITutorialManager
    {
        //Internal Interface References
        private IGameManager gameManager;
        private IUIManager uiManager;
        private ITutorialUI tutorialUI;
        //Tutorial Interface References
        [SerializeField]
        private GameObject cameraTutorialObject;
        public ICameraTutorial CameraTutorial { get; protected set; }
        private List<ITutorial> tutorials = new List<ITutorial>();
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;
        private bool componentMissing;
        private bool started = false;
        private GameSettings gameSettings;
        private int tutorialCounter = 0;
        const int CameraPlacement = 0;


        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            gameSettings = FindObjectOfType<GameSettings>();
            if (interfaceFactory == null || gameSettings == null)
                gameObject.SetActive(false);
            try
            {
                CameraTutorial = cameraTutorialObject.GetComponent(typeof(ICameraTutorial)) as ICameraTutorial;
                if (CameraTutorial == null)
                    componentMissing = true;
            }
            catch
            {
                componentMissing = true;
            }
        }

        void Start()
        {
            errorHandler = interfaceFactory.ErrorHandler;
            gameManager = interfaceFactory.GameManager;
            uiManager = interfaceFactory.UIManager;
            gameManager = interfaceFactory.GameManager;
            if (gameManager == null || uiManager == null || errorHandler == null)
                gameObject.SetActive(false);
            else
            {
                tutorialUI = uiManager.TutorialUI;
                if (tutorialUI == null)
                    errorHandler.ReportError("Tutorial UI missing", ErrorState.restart_scene);
                else
                    started = true;
            }
        }

        public void BeginTutorial()
        {
            if (!started)
                Start();
            if (gameObject.activeSelf)
            {
                InitializeTutorialList();
                tutorialUI.SetMainText("Welcome to the Tutorial!");
                tutorialUI.EnableButton1(false);
                tutorialUI.EnableButton2(true);
                tutorialUI.SetButton2Text("Start");
                tutorialUI.SetButton2Delegate(tutorials[0].StartTutorial);
            }
        }
  
        private void InitializeTutorialList()
        {
            tutorials.Insert(CameraPlacement, CameraTutorial);
        }

        private void StartNextTutorial()
        {
            ITutorial nextTutorial = tutorials[tutorialCounter];
            nextTutorial.StartTutorial();
        }

        public void CurrentTutorialFinished()
        {
            tutorialCounter++;
            if (tutorialCounter >= tutorials.Count)
                EndTutorial();
            else
                StartNextTutorial();
        }

        private void EndTutorial()
        {
            gameSettings.TutorialActive = false;
            tutorialUI.SetUIPosition(TextAnchor.MiddleCenter);
            tutorialUI.SetMainText("Tutorial finished!");
            tutorialUI.EnableButton1(true);
            tutorialUI.EnableButton2(true);
            tutorialUI.SetButton1Text("Exit");
            tutorialUI.SetButton2Text("Play Game");
            tutorialUI.SetButton1Delegate(gameManager.ReturnToMainMenu);
            tutorialUI.SetButton2Delegate(gameManager.GameReset);
        }
    }
}


