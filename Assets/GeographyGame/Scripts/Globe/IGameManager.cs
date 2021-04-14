namespace WPM
{
    public interface IGameManager
    {
        ICameraManager CameraManager { get; }
        ITouristManager TouristManager { get; }
        ITurnsManager TurnsManager { get; }
        IScoreManager ScoreManager { get; }
        ISelectableObject HighlightedObject { get; set; }
        IPlayerCharacter Player { get; set; }
        ISelectableObject SelectedObject { get; set; }
        bool GamePaused { get; set; }

        void ExitGame();
        void GameOver();
        void GameReset();
        void ResumeGame();
    }
}