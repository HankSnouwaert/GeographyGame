﻿using System.Collections;
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
        //Error Checking
        private InterfaceFactory interfaceFactory;
        private IErrorHandler errorHandler;
        private bool componentMissing;
        private bool started = false;
        private GameSettings gameSettings;

        private void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            gameSettings = FindObjectOfType<GameSettings>();
            if (interfaceFactory == null || gameSettings == null)
                gameObject.SetActive(false);
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
                tutorialUI.SetMainText("Welcome to the Tutorial!");
                tutorialUI.EnableButton1(false);
                tutorialUI.EnableButton2(true);
                tutorialUI.SetButton2Text("Start");
                tutorialUI.SetButton2Delegate(EndTutorial);
            }
        }

        public void EndTutorial()
        {
            gameSettings.TutorialActive = false;
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


