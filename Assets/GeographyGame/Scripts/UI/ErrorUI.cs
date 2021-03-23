using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{

    public class ErrorUI : UIElement, IErrorUI
    {
        private Text errorMessage;
        private InputField stackTraceInputField;
        private Button errorButton;
        private IErrorHandler errorHandler;

        public override void Awake()
        {
            base.Awake();
            errorHandler = gameManager.ErrorHandler;
            Transform textObject = uiObject.transform.GetChild(0);
            uiObject.SetActive(false);
            textObject = uiObject.transform.GetChild(0);
            errorMessage = textObject.gameObject.GetComponent(typeof(Text)) as Text;
            Transform scrollViewTextObject = uiObject.transform.GetChild(1).GetChild(0).GetChild(0);
            stackTraceInputField = scrollViewTextObject.gameObject.GetComponent(typeof(InputField)) as InputField;
            Transform buttonObject = uiObject.transform.GetChild(2);
            errorButton = buttonObject.gameObject.GetComponent(typeof(Button)) as Button;
        }

        public override void CloseUI()
        {
            base.CloseUI();
            setErrorMessage("");
            setStackTrace("");
        }

        public void setErrorMessage(string message)
        {
            errorMessage.text = message;
        }

        public void setStackTrace(string stackTrace)
        {
            stackTraceInputField.text = stackTrace;
        }

        public void errorUIClosed()
        {
            errorHandler.errorResponse();
        }

    }

}