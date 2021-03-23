namespace WPM
{
    /// <summary>
    ///  Interface that holds interfaces for all GUI elements
    /// </summary>
    public interface IUIManager
    {
        bool CursorOverUI { get; set; }
        IErrorUI ErrorUI { get; set; }
    }
}