using UnityEngine;

namespace WPM
{
    /// <summary>
    ///  Base interface for any GUI panel
    /// </summary>
    public interface IUIElement
    {
        /// <summary>
        ///  The UI's game object
        /// </summary>
        GameObject UIObject { get; }
        
        /// <summary>
        ///  A flag indicating if the UI is open or not
        /// </summary>
        bool UIOpen { get; set; }

        /// <summary>
        ///  Closes the UI
        /// </summary>
        void CloseUI();

        /// <summary>
        ///  Opens the UI
        /// </summary>
        void OpenUI();
    }
}