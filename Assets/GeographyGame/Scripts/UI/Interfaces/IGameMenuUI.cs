namespace WPM
{
    /// <summary> 
    /// UI used to display in game menu
    /// </summary>
    public interface IGameMenuUI : IUIElement
    {
        /// <summary> 
        /// Executed if the player chooses to exit the game
        /// </summary>
        void ExitGameSelected();

        /// <summary> 
        /// Executed if the player chooses to restart the game
        /// </summary>
        void RestartGameSelected();

        /// <summary> 
        /// Executed if the player chooses to return to the game
        /// </summary>
        void ReturnToGameSelected();
    }
}