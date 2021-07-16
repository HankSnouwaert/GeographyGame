using System.Collections.Generic;

namespace WPM
{
    /// <summary> 
    /// Interface used to hold general game state information and access to manager and player interfaces
    /// </summary>
    public interface IGameManager
    {
        /// <summary> 
        /// Interface used to intantiate and access the player object
        /// </summary>
        IPlayerManager PlayerManager { get; }
        
        /// <summary> 
        /// Interface used to access in-game camera functionality
        /// </summary>
        ICameraManager CameraManager { get; }
        
        /// <summary> 
        /// Interface used to access tourist functionality
        /// </summary>
        ITouristManager TouristManager { get; }
        
        /// <summary> 
        /// Interface used to access game turns
        /// </summary>
        ITurnsManager TurnsManager { get; }
        
        /// <summary> 
        /// Interface used to access game score
        /// </summary>
        IScoreManager ScoreManager { get; }

        /// <summary> 
        /// Interface used to access tutorial manager
        /// </summary>
        ITutorialManager TutorialManager { get; }

        /// <summary> 
        /// Object the cursor is currently over
        /// </summary>
        ISelectableObject HighlightedObject { get; set; }

        /// <summary> 
        /// The selectable objects currently selected
        /// </summary>
        List<ISelectableObject> SelectedObjects { get; }

        /// <summary> 
        /// Flag that indicates if the game is paused
        /// </summary>
        bool GamePaused { get; set; }

        /// <summary> 
        /// Selects a given selectable object
        /// </summary>
        /// <param name="newlySelectedObject"> The object being selected </param>
        void ObjectSelected(ISelectableObject newlySelectedObject);

        /// <summary> 
        /// Deselects a given selectable object
        /// </summary>
        /// <param name="delectedObject"> The object being deselected </param>
        void ObjectDeselected(ISelectableObject deselectedObject);

        /// <summary> 
        /// Exit the application
        /// </summary>
        void ExitGame();

        /// <summary> 
        /// Return to the game's main menu
        /// </summary>
        void ReturnToMainMenu();

        /// <summary> 
        /// End the game
        /// </summary>
        void GameOver();
        
        /// <summary> 
        /// Restart the game
        /// </summary>
        void GameReset();
        
        /// <summary> 
        /// Resume the game if it is paused
        /// </summary>
        void ResumeGame();
    }
}