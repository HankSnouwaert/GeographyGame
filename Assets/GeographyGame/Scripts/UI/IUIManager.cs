
namespace WPM
{
    /// <summary>
    ///  Interface that holds interfaces for all GUI elements
    /// </summary>
    public interface IUIManager
    {
        bool CursorOverUI { get; set; }
        bool ClosingUI { get; set; }
        INavigationUI NavigationUI { get; set; }
        IDropOffUI DropOffUI { get; set; }
        IScoreUI ScoreUI { get; set; }
        ITurnsUI TurnsUI { get; set; }
        IErrorUI ErrorUI { get; set; }
        IMouseOverInfoUI MouseOverInfoUI { get; set; }
        IGameOverUI GameOverUI { get; set; }
        IGameMenuUI GameMenuUI { get; set; }
        void GameOver();
    }
}