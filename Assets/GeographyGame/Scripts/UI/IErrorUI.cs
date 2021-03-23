namespace WPM
{
    public interface IErrorUI : IUIElement
    {
        void errorUIClosed();
        void setErrorMessage(string message);
        void setStackTrace(string stackTrace);
    }
}