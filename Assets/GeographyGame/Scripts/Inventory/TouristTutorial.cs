using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{

    public class TouristTutorial : Tutorial, ITouristTutorial
    {
        //Private Interface References
        private IGameManager gameManager;
        private ITouristManager touristManager;
        private ITutorialManager tutorialManager;
        private IUIManager uiManager;
        private ITutorialUI tutorialUI;
        //Local Variables
        private int tutorialCounter = -1;
        const int SelectTourist = 0;
        const int NumberOfTutorials = 1;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;

        //Temp Var
        private bool touristSelected = false;

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if (interfaceFactory == null)
                gameObject.SetActive(false);
        }

        void Start()
        {
            errorHandler = interfaceFactory.ErrorHandler;
            gameManager = interfaceFactory.GameManager;
            uiManager = interfaceFactory.UIManager;
            if (uiManager == null || errorHandler == null || gameManager == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                touristManager = gameManager.TouristManager;
                if (touristManager == null)
                    errorHandler.ReportError("Tourist Manager Missing", ErrorState.close_application);
                tutorialManager = gameManager.TutorialManager;
                if (tutorialManager == null)
                    errorHandler.ReportError("Tutorial Manager Missing", ErrorState.close_application);
                tutorialUI = uiManager.TutorialUI;
                if (tutorialUI == null)
                    errorHandler.ReportError("Tutorial UI Missing", ErrorState.close_application);
            }
        }

        void Update()
        {
            switch (tutorialCounter)
            {
                case (SelectTourist):
                    if (touristManager.TouristSelected)
                    {
                        touristManager.ActiveTutorial = 0;
                        NextTutorial();
                    }
                    break;

                default:
                    break;
            }
        }

        public override void StartTutorial()
        {
            touristManager.ActiveTutorial = 1;
            StartTouristTutorial();
        }

        private void StartTouristTutorial()
        {
            tutorialUI.SetUIPosition(TextAnchor.UpperRight);
            tutorialUI.SetMainText("This is the tourist tutorial");
            tutorialUI.SetButton1Text("Next");
            tutorialUI.SetButton1Delegate(NextTutorial);
            tutorialUI.EnableButton1(true);
            tutorialUI.EnableButton2(false);
        }

        private void NextTutorial()
        {
            tutorialCounter++;
            switch (tutorialCounter)
            {
                case (SelectTourist):
                    SelectTouristTutorial();
                    break;

                case (NumberOfTutorials):
                    EndTutorial();
                    break;

                default:
                    break;
            }
        }

        private void SelectTouristTutorial()
        {
            touristManager.ActiveTutorial = 1;
            tutorialUI.SetUIPosition(TextAnchor.UpperRight);
            tutorialUI.SetMainText("Select the Tourist");
            tutorialUI.SetButton1Delegate(SelectTouristTemp);
            tutorialUI.EnableButton1(false);
            tutorialUI.EnableButton2(false);
            touristManager.GenerateTourist();
        }

        //Temp Function
        private void SelectTouristTemp()
        {
            touristSelected = true;
        }

        public void EndTutorial()
        {
            tutorialCounter = -1;
            tutorialManager.CurrentTutorialFinished();
        }
    }
}