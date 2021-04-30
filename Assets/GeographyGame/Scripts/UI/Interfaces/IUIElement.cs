using UnityEngine;

namespace WPM
{
    /// <summary>
    ///  Base interface for any GUI panel
    /// </summary>
    public interface IUIElement
    {
        GameObject UIObject { get; }
        bool UIOpen { get; set; }
        void CloseUI();
        void OpenUI();
    }
}