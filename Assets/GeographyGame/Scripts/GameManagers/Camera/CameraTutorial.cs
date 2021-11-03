using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class CameraTutorial : Tutorial, ICameraTutorial
    {
        //Private Interface References
        private IGameManager gameManager;
        private ICameraManager cameraManager;
        private ITutorialManager tutorialManager;
        private IUIManager uiManager;
        private ITutorialUI tutorialUI;
        //Local Variables
        private int tutorialCounter = -1;
        const int CameraMovePlacement = 0;
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;

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
                cameraManager = gameManager.CameraManager;
                if(cameraManager == null)
                    errorHandler.ReportError("Camera Manager Missing", ErrorState.close_application);
                tutorialManager = gameManager.TutorialManager;
                if(tutorialManager == null)
                    errorHandler.ReportError("Tutorial Manager Missing", ErrorState.close_application);
                tutorialUI = uiManager.TutorialUI;
                if(tutorialUI == null)
                    errorHandler.ReportError("Tutorial UI Missing", ErrorState.close_application);
            }
        }

        void Update()
        {
            switch(tutorialCounter)
            {
                case (CameraMovePlacement):

                    break;

                default:
                    break;

            }
        }

        public override void StartTutorial()
        {
            StartCameraMoveTutorial();
        }

        public void EndTutorial()
        {
            tutorialCounter = -1;
            tutorialManager.CurrentTutorialFinished();
        }

        public void StartCameraMoveTutorial()
        {
            tutorialUI.SetUIPosition(TextAnchor.UpperRight);
            tutorialUI.SetMainText("This is the camera tutorial");
            tutorialUI.SetButton1Delegate(EndTutorial);
            tutorialUI.EnableButton2(false);
        }

    }
}
