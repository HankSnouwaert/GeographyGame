using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class CameraTutorial : MonoBehaviour, ICameraTutorial
    {
        //Private Interface References
        private IGameManager gameManager;
        private ICameraManager cameraManager;
        private ITutorialManager tutorialManager;
        private IUIManager uiManager;
        private ITutorialUI tutorialUI;
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

        }

        public void StartCameraMoveTutorial()
        {

        }

    }
}
