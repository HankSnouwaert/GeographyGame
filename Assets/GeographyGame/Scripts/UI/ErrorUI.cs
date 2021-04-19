using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{

    public class ErrorUI : MonoBehaviour, IErrorUI
    {
        public bool UIOpen { get; set; }
        private Text errorMessage;
        private InputField stackTraceInputField;
        private Button errorButton;
        private IErrorHandler errorHandler;
        private IUIManager uiManager;
        private bool errorUIAwake = false;
        private bool errorUIStarted = false;
        private InterfaceFactory interfaceFactory;
        private bool componentMissing = false;

        public void Awake()
        {
            interfaceFactory = FindObjectOfType<InterfaceFactory>();
            if(interfaceFactory == null)
                gameObject.SetActive(false);
            try
            {
                Transform textObject = gameObject.transform.GetChild(0);
                textObject = gameObject.transform.GetChild(0);
                errorMessage = textObject.gameObject.GetComponent(typeof(Text)) as Text;
                Transform scrollViewTextObject = gameObject.transform.GetChild(1).GetChild(0).GetChild(0);
                stackTraceInputField = scrollViewTextObject.gameObject.GetComponent(typeof(InputField)) as InputField;
                Transform buttonObject = gameObject.transform.GetChild(2);
                errorButton = buttonObject.gameObject.GetComponent(typeof(Button)) as Button;
                errorUIAwake = true;
            }
            catch
            {
                componentMissing = true;
            } 
        }

        public void Start()
        {
            if (!errorUIAwake)
                Awake();
            errorHandler = interfaceFactory.ErrorHandler;
            uiManager = interfaceFactory.UIManager;  //UI Manager is optional
            if (errorHandler == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                errorUIStarted = true;
                if (componentMissing)
                    errorHandler.ReportError("Error UI component missing", ErrorState.restart_scene);
            }
            
        }

        public void OpenUI()
        {
            gameObject.SetActive(true);
            UIOpen = true;
        }

        public void CloseUI()
        {
            if (!errorUIStarted)
                Start();

            gameObject.SetActive(false);
            setErrorMessage("");
            setStackTrace("");
            UIOpen = false;

            if (uiManager != null)
                uiManager.ClosingUI = true;
        }

        public void setErrorMessage(string message)
        {
            if (!errorUIStarted)
                Start();
            if(errorMessage != null)
                errorMessage.text = message;
            else
                errorHandler.EmergencyExit("Error Message componenent missing");
        }

        public void setStackTrace(string stackTrace)
        {
            if (!errorUIStarted)
                Start();
            if(stackTraceInputField != null)
                stackTraceInputField.text = stackTrace;
            else
                errorHandler.EmergencyExit("Stack Trace componenent missing");
        }

        public void errorUIClosed()
        {
            if (errorHandler != null)
                errorHandler.ErrorResponse();
            else
                gameObject.SetActive(false);
        }
    }

}