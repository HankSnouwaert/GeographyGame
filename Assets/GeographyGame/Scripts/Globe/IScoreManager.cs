namespace WPM
{
    public interface IScoreManager
    {
        int Score { get; }

        void UpdateScore(int scoreModification);
    }
}