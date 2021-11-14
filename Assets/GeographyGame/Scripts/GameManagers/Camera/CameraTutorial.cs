
using System.Collections;
using System;
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
        const int CenterCamera = 1;
        const int NumberOfTutorials = 2;
        float lastCameraPositionMagnitude = 0;
        const double cameraMovementThreshhold = 0.1;
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

                    if (Math.Abs(Camera.main.transform.position.z - lastCameraPositionMagnitude) >= cameraMovementThreshhold)
                        NextTutorial();
                    break;

                case (CenterCamera):
                    if(TutorialActionComplete)
                        NextTutorial();
                    break;

                default:
                    break;
            }
        }

        public override void StartTutorial()
        {
            cameraManager.ActiveTutorial = 1;
            StartCameraMoveTutorial();
        }

        private void NextTutorial()
        {
            tutorialCounter++;
            switch(tutorialCounter)
            {
                case (CameraMovePlacement):
                    MoveTheCamera();
                    break;

                case (CenterCamera):
                    CenterTheCamera();
                    break;

                case (NumberOfTutorials):
                    EndTutorial();
                    break;

                default:
                    break;
            }
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
            tutorialUI.SetButton1Text("Next");
            tutorialUI.SetButton1Delegate(NextTutorial);
            tutorialUI.EnableButton1(true);
            tutorialUI.EnableButton2(false);
        }

        public void MoveTheCamera()
        {
            tutorialUI.SetUIPosition(TextAnchor.UpperRight);
            tutorialUI.SetMainText("Try moving the camera");
            tutorialUI.EnableButton1(false);
            tutorialUI.EnableButton2(false);
            tutorialCounter = CameraMovePlacement;
            lastCameraPositionMagnitude = Camera.main.transform.position.z;
        }

        public void CenterTheCamera()
        {
            tutorialUI.SetUIPosition(TextAnchor.UpperRight);
            tutorialUI.SetMainText("Try centering the camera by pressing the spacebar");
            tutorialUI.EnableButton1(false);
            tutorialUI.EnableButton2(false);
            tutorialCounter = CenterCamera;
        }

    }
}
