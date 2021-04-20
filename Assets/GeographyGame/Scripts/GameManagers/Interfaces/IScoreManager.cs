namespace WPM
{
    /// <summary>
    ///  Maintains and updates the player's total score
    /// </summary>
    public interface IScoreManager
    {
        /// <summary>
        ///  The player's total score
        /// </summary>
        int Score { get; }

        /// <summary>
        ///  Update the games current score
        /// </summary>
        /// <param name="scoreModification"></param> The amount the score should be changed by>
        /// <returns></returns> 
        void UpdateScore(int scoreModification);
    }
}