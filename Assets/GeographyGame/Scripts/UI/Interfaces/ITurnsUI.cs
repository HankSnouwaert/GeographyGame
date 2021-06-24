namespace WPM
{
    /// <summary> 
    /// UI used to display the remaining number of turns to the player
    /// </summary>
    public interface ITurnsUI : IUIElement
    {
        /// <summary> 
        /// Updates the remaining number of turns to the player
        /// </summary>
        /// <param name="turnsRemaining"> The new number of turns being displayed to the player </param>
        void UpdateDisplayedRemainingTurns(int turnsRemaining);
    }
}