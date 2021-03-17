using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WPM
{

    public class ErrorPanel : GUIPanel
    {
        private Text errorMessage;
        private InputField stackTraceInputField;
        private Button errorButton;
        public ErrorHandler errorHandler;



        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            Transform textObject = panel.transform.GetChild(0);
            panel.SetActive(false);
            textObject = panel.transform.GetChild(0);
            errorMessage = textObject.gameObject.GetComponent(typeof(Text)) as Text;
            Transform scrollViewTextObject = panel.transform.GetChild(1).GetChild(0).GetChild(0);
            stackTraceInputField = scrollViewTextObject.gameObject.GetComponent(typeof(InputField)) as InputField;
            Transform buttonObject = panel.transform.GetChild(2);
            errorButton = buttonObject.gameObject.GetComponent(typeof(Button)) as Button;
        }

        public void setErrorMessage(string message)
        {
            errorMessage.text = message;
        }

        public void setStackTrace(string stackTrace)
        {
            stackTraceInputField.text = stackTrace;
        }

        public void errorButtonPressed()
        {
            errorHandler.errorResponse();
        }

    }

}