namespace WPM
{
    /// <summary> 
    /// UI used to display the score to the player
    /// </summary>
    public interface IScoreUI : IUIElement
    {
        /// <summary> 
        /// Updates the displayed score
        /// </summary>
        /// <param name="score"> The new score being displayed </param>
        void UpdateDisplayedScore(int score);
    }
}