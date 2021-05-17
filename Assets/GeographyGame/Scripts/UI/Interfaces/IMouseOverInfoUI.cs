namespace WPM
{
    /// <summary> 
    /// UI used to display information relevant to whatever the player's curser has moved over
    /// </summary>
    public interface IMouseOverInfoUI
    {
        /// <summary> 
        /// The string of information to display to the player
        /// </summary>
        string MouseOverInfoString { get; set; }

        /// <summary> 
        /// Builds a string of information for display to the player
        /// </summary>
        /// <param name="province"> The province to be described to the player</param>
        /// <param name="country"> The country to be described to the player</param>
        /// <param name="highlightedObject"> The object on the map to be described to the player</param>
        string CreateMouseOverInfoString(Province province, Country country, IMappableObject highlightedObject);

        /// <summary> 
        /// Sets the text to be displayed to the player
        /// </summary>
        /// <param name="textToSet"> The text to be displayed using this UI </param>
        void SetMouseOverInfoMessage(string textToSet);

        /// <summary> 
        /// Checks what the player's cursor is highlighting, and updates the Mouse Over Info UI accordingly
        /// </summary>
        void UpdateUI();
    }
}