namespace WPM
{
    /// <summary>
    ///  Interface that holds interfaces for all GUI elements
    /// </summary>
    public interface IUIManager
    {
        bool CursorOverUI { get; set; }
        bool ClosingUI { get; set; }
        IErrorUI ErrorUI { get; set; }
        IMouseOverInfoUI MouseOverInfoUI { get; set; }
    }
}