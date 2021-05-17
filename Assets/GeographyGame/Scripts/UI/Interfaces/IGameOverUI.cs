namespace WPM
{
    /// <summary> 
    /// UI used to display Game Over information and options
    /// </summary>
    public interface IGameOverUI :IUIElement
    {
        /// <summary> 
        /// Executed if the player chooses to exit the game
        /// </summary>
        void ExitGameSelected();
        /// <summary> 
        /// 
        /// Executed if the player chooses to restart the game
        /// </summary>
        void ResetGameSelected();
    }
}