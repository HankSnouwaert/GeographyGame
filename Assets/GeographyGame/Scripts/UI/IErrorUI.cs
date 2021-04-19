namespace WPM
{
    public interface IErrorUI 
    {
        void errorUIClosed();
        void setErrorMessage(string message);
        void setStackTrace(string stackTrace);
        void OpenUI();
        void CloseUI();

    }
}