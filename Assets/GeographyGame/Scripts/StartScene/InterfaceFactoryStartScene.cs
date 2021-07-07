using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    /// <summary> 
    /// Used to store references to the highest level game objects
    /// </summary>
    public class InterfaceFactoryStartScene : MonoBehaviour
    {
        
        //[SerializeField]
        //private GameObject uiManagerObject;
        [SerializeField]
        private GameObject errorHandlerObject;

        public IUIManager UIManager { get; protected set; }
        public IErrorHandler ErrorHandler { get; protected set; }

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
            //if (uiManagerObject == null)
            //    ErrorHandler.EmergencyExit("UI Manager Object Missing");
            /*
            //Get critical components
            UIManager = uiManagerObject.GetComponent(typeof(IUIManager)) as IUIManager;
            if (UIManager == null)
            {
                ErrorHandler.ReportError("UI Manager Component Missing", ErrorState.close_application);
                DisableMainComponents();
            }
            */
        }

        /// <summary> 
        /// Disable all game objects to prevent further error
        /// </summary>
        /*
        public void DisableMainComponents()
        {
            uiManagerObject.SetActive(false);
        }
        */
    }
}