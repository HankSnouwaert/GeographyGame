﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class InterfaceFactory : MonoBehaviour
    {
        public GameObject gameManagerObject;
        public IGameManager GameManager { get; set; }
        public GameObject uiManagerObject;
        public IUIManager UIManager { get; set; }
        public GameObject errorHandlerObject;
        public IErrorHandler ErrorHandler { get; set; }
        public GameObject globeManagerObject;
        public IGlobeManager GlobeManager { get; set; }

        void Awake()
        {
            //Get error handler
            if (errorHandlerObject == null)
            {
                Debug.Log("Error Handler Object Missing");
                Application.Quit();
            }
            ErrorHandler = errorHandlerObject.GetComponent(typeof(IErrorHandler)) as IErrorHandler;
            if (ErrorHandler == null)
            {
                Debug.Log("Error Handler Component Missing");
                Application.Quit();
            }

            //Check for critical game objects
            if (gameManagerObject == null)
                ErrorHandler.EmergencyExit("Game Manager Object Missing");
            if (uiManagerObject == null)
                ErrorHandler.EmergencyExit("UI Manager Object Missing");
            if (globeManagerObject == null)
                ErrorHandler.EmergencyExit("Globe Manager Object Missing");

            //Get critical components
            GameManager = gameManagerObject.GetComponent(typeof(IGameManager)) as IGameManager;
            if (GameManager == null)
            {
                ErrorHandler.ReportError("Game Manager Component Missing", ErrorState.close_application);
                DisableMainComponents();
            }
            UIManager = uiManagerObject.GetComponent(typeof(IUIManager)) as IUIManager;
            if (UIManager == null)
            {
                ErrorHandler.ReportError("UI Manager Component Missing", ErrorState.close_application);
                DisableMainComponents();
            }    
            GlobeManager = globeManagerObject.GetComponent(typeof(IGlobeManager)) as IGlobeManager;
            if (GlobeManager == null)
            {
                ErrorHandler.ReportError("Globe Manager Component Missing", ErrorState.close_application);
                DisableMainComponents();
            }
        }

        /// <summary> 
        /// Disable all game objects to prevent further error
        /// </summary>
        public void DisableMainComponents()
        {
            gameManagerObject.SetActive(false);
            uiManagerObject.SetActive(false);
            globeManagerObject.SetActive(false);
        }

    }
}
