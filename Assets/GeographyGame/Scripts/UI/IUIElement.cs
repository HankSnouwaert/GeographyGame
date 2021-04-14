namespace WPM
{
    /// <summary>
    ///  Base interface for any GUI panel
    /// </summary>
    public interface IUIElement
    {
        bool UIOpen { get; set; }
        void CloseUI();
        void OpenUI();
    }
}