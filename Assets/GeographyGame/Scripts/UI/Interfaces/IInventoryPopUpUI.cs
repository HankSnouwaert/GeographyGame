namespace WPM
{
    /// <summary> 
    /// UI used to display popup information relating to inventory items
    /// </summary>
    public interface IInventoryPopUpUI : IUIElement
    {
        /// <summary> 
        /// Flag used to determine whether the current popup will remain open after the player moves the cursor away 
        /// </summary>
        bool TempPopUp { get; set; }

        /// <summary> 
        /// Used to display the inventory pop up UI
        /// </summary>
        /// <param name="displayString"> The string to be displayed to the player</param>
        /// <param name="persistant"> Whether the pop up should remain open after the player moves the cursor away</param>
        void DisplayPopUp(string displayString, bool persistant);

        /// <summary> 
        /// Used to reset the pop up to display whatever the last persistant message was
        /// </summary>
        void ResetPersistantMessage();

        /// <summary> 
        /// Used to clear the current pop up message
        /// </summary>
        /// <param name="persistant"> Persistant indicates if the last persistant message set should be cleared </param>
        void ClearPopUp(bool persistant);
    }
}