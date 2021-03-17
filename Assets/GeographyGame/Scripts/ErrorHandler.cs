using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPM
{
    public class ErrorHandler : MonoBehaviour
    {
        public GameManager gameManager;
        private System.Exception exception;
        private string errorMessage;
        private ErrorState errorState = ErrorState.no_error;
        public ErrorPanel errorPanel;

        // Start is called before the first frame update
        void Awake()
        {
            errorPanel.errorHandler = this;
        }

        public void reportError(string message, ErrorState state)
        {
            errorState = state;
            errorPanel.setErrorMessage(message);
            errorPanel.OpenPanel();
        }

        public void catchException(System.Exception ex)
        {
            string combinedStackTrace = ex.StackTrace;
            var inner = ex.InnerException;
            while (inner != null)
            {
                combinedStackTrace = combinedStackTrace + inner.StackTrace;
                inner = inner.InnerException;
            }
            errorState = ErrorState.close_window;
            errorPanel.setErrorMessage(ex.Message);
            errorPanel.setStackTrace(combinedStackTrace);
            errorPanel.OpenPanel();
        }

        public void errorResponse()
        {
            switch (errorState)
            {
                case (ErrorState.close_window):
                    errorPanel.ClosePanel();
                    break;

                case (ErrorState.restart_scene):
                    gameManager.GameReset();
                    break;

                case (ErrorState.close_application):
                    gameManager.ExitGame();
                    break;

                default:
                    gameManager.ExitGame();
                    break;
            }
                
        }

    }
}
