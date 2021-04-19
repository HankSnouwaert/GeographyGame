namespace WPM
{
    public interface IErrorUI 
    {
        /// <summary>
        ///  Executed when the user closes the error UI
        /// </summary>
        void errorUIClosed();
        
        /// <summary>
        ///  Sets the error message displayed by the error UI
        ///  <param name="message"></param> The error message for the user>
        /// </summary>
        void setErrorMessage(string message);
        
        /// <summary>
        ///  Sets the stack trace displayed by the error UI
        ///  <param name="stackTrace"></param> The stack trace displayed to the user>
        /// </summary>
        void setStackTrace(string stackTrace);

        /// <summary>
        ///  Opens the error UI
        /// </summary>
        void OpenUI();

        /// <summary>
        ///  Closes the error UI
        /// </summary>
        void CloseUI();

    }
}