
namespace WPM
{
    /// <summary>
    ///  Interface that holds interfaces for all GUI elements
    /// </summary>
    public interface IUIManager
    {
        /// <summary>
        ///  Flag indicating if the cursor is being block by a UI element
        /// </summary>
        bool CursorOverUI { get; set; }
        /// <summary>
        ///  Flag indicating if a UI is being closed
        /// </summary>
        bool ClosingUI { get; set; }
        /// <summary>
        ///  Displays information relating to the player's current location
        /// </summary>
        INavigationUI NavigationUI { get; }
        /// <summary>
        ///  UI used to drop off tourists
        /// </summary>
        IDropOffUI DropOffUI { get; }
        /// <summary>
        ///  Displays the game's score
        /// </summary>
        IScoreUI ScoreUI { get; }
        /// <summary>
        ///  Displays the game's remaing turns
        /// </summary>
        ITurnsUI TurnsUI { get; }
        /// <summary>
        ///  Displayes information relating to what the cursor is over
        /// </summary>
        IMouseOverInfoUI MouseOverInfoUI { get; }
        /// <summary>
        ///  UI that displays when the game ends
        /// </summary>
        IGameOverUI GameOverUI { get; }
        /// <summary>
        ///  The main in game menu UI
        /// </summary>
        IGameMenuUI GameMenuUI { get; }
        /// <summary>
        ///  The pop up that appears when the cursor is moved over items in the inventory
        /// </summary>
        IInventoryPopUpUI InventoryPopUpUI { get; }
        /// <summary>
        ///  Sets up UIs for end of game display
        /// </summary>
        void GameOver();
    }
}